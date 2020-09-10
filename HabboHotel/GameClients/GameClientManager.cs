﻿using System;
using System.Collections.Generic;
using System.Text;
using Plus.Core;
using Plus.HabboHotel.Users.Messenger;
using System.Linq;
using System.Collections.Concurrent;
using Plus.Communication.Packets.Outgoing;
using log4net;
using System.Data;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.Database.Interfaces;
using System.Collections;
using Plus.Communication.Packets.Outgoing.Handshake;
using System.Diagnostics;
using DotNetty.Transport.Channels;

namespace Plus.HabboHotel.GameClients
{
    public class GameClientManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GameClientManager));

        private ConcurrentDictionary<IChannelId, GameClient> _clients;
        private ConcurrentDictionary<int, GameClient> _userIDRegister;
        private ConcurrentDictionary<string, GameClient> _usernameRegister;

        private readonly Queue timedOutConnections;

        private readonly Stopwatch clientPingStopwatch;

        public GameClientManager()
        {
            this._clients = new ConcurrentDictionary<IChannelId, GameClient>();
            this._userIDRegister = new ConcurrentDictionary<int, GameClient>();
            this._usernameRegister = new ConcurrentDictionary<string, GameClient>();

            timedOutConnections = new Queue();

            clientPingStopwatch = new Stopwatch();
            clientPingStopwatch.Start();
        }

        public void OnCycle()
        {
            TestClientConnections();
            HandleTimeouts();
        }

        public GameClient GetClientByUserId(int userID)
        {
            if (_userIDRegister.ContainsKey(userID))
                return _userIDRegister[userID];
            return null;
        }

        public GameClient GetClientByUsername(string username)
        {
            if (_usernameRegister.ContainsKey(username.ToLower()))
                return _usernameRegister[username.ToLower()];
            return null;
        }

        public bool TryGetClient(IChannelId ClientId, out GameClient Client)
        {
            return this._clients.TryGetValue(ClientId, out Client);
        }

        public bool UpdateClientUsername(GameClient Client, string OldUsername, string NewUsername)
        {
            if (Client == null || !_usernameRegister.ContainsKey(OldUsername.ToLower()))
                return false;

            _usernameRegister.TryRemove(OldUsername.ToLower(), out Client);
            _usernameRegister.TryAdd(NewUsername.ToLower(), Client);
            return true;
        }

        public string GetNameById(int Id)
        {
            GameClient client = GetClientByUserId(Id);

            if (client != null)
                return client.GetHabbo().Username;

            string username;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT username FROM users WHERE id = @id LIMIT 1");
                dbClient.AddParameter("id", Id);
                username = dbClient.GetString();
            }

            return username;
        }

        public IEnumerable<GameClient> GetClientsById(Dictionary<int, MessengerBuddy>.KeyCollection users)
        {
            foreach (int id in users)
            {
                GameClient client = GetClientByUserId(id);
                if (client != null)
                    yield return client;
            }
        }

        public void StaffAlert(MessageComposer Message, int Exclude = 0)
        {
            foreach (GameClient client in this.GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                    continue;

                if (client.GetHabbo().Rank < 2 || client.GetHabbo().Id == Exclude)
                    continue;

                client.SendPacket(Message);
            }
        }

        public void ModAlert(string Message)
        {
            foreach (GameClient client in this.GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                    continue;

                if (client.GetHabbo().GetPermissions().HasRight("mod_tool") &&
                    !client.GetHabbo().GetPermissions().HasRight("staff_ignore_mod_alert"))
                {
                    try
                    {
                        client.SendWhisper(Message, 5);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public void DoAdvertisingReport(GameClient Reporter, GameClient Target)
        {
            if (Reporter == null || Target == null || Reporter.GetHabbo() == null || Target.GetHabbo() == null)
                return;

            StringBuilder Builder = new StringBuilder();
            Builder.Append("New report submitted!\r\r");
            Builder.Append("Reporter: " + Reporter.GetHabbo().Username + "\r");
            Builder.Append("Reported User: " + Target.GetHabbo().Username + "\r\r");
            Builder.Append(Target.GetHabbo().Username + "s last 10 messages:\r\r");

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `message` FROM `chatlogs` WHERE `user_id` = '" + Target.GetHabbo().Id +
                                  "' ORDER BY `id` DESC LIMIT 10");
                int Number = 11;
                var reader = dbClient.GetTable();
                foreach (DataRow row in reader.Rows)
                {
                    Number -= 1;
                    Builder.Append(Number + ": " + row["message"] + "\r");
                }
            }

            foreach (GameClient Client in this.GetClients.ToList())
            {
                if (Client == null || Client.GetHabbo() == null)
                    continue;

                if (Client.GetHabbo().GetPermissions().HasRight("mod_tool") && !Client.GetHabbo().GetPermissions()
                        .HasRight("staff_ignore_advertisement_reports"))
                    Client.SendPacket(new MotdNotificationComposer(Builder.ToString()));
            }
        }


        public void SendPacket(MessageComposer Packet, string fuse = "")
        {
            foreach (GameClient Client in this._clients.Values.ToList())
            {
                if (Client == null || Client.GetHabbo() == null)
                    continue;

                if (!string.IsNullOrEmpty(fuse))
                {
                    if (!Client.GetHabbo().GetPermissions().HasRight(fuse))
                        continue;
                }

                Client.SendPacket(Packet);
            }
        }

        public void CreateAndStartClient(IChannelHandlerContext connection)
        {
            GameClient Client = new GameClient(connection);
            if (this._clients.TryAdd(connection.Channel.Id, Client))
            {
                //Hmmmmm?
            }
            else
                connection.CloseAsync();
        }

        public void DisposeConnection(IChannelId clientID)
        {
            if (!TryGetClient(clientID, out GameClient Client))
                return;

            if (Client != null)
                Client.Dispose();

            this._clients.TryRemove(clientID, out Client);
        }

        public void LogClonesOut(int UserID)
        {
            GameClient client = GetClientByUserId(UserID);
            if (client != null)
                client.Disconnect();
        }

        public void RegisterClient(GameClient client, int userID, string username)
        {
            if (_usernameRegister.ContainsKey(username.ToLower()))
                _usernameRegister[username.ToLower()] = client;
            else
                _usernameRegister.TryAdd(username.ToLower(), client);

            if (_userIDRegister.ContainsKey(userID))
                _userIDRegister[userID] = client;
            else
                _userIDRegister.TryAdd(userID, client);
        }

        public void UnregisterClient(int userid, string username)
        {
            _userIDRegister.TryRemove(userid, out GameClient Client);
            _usernameRegister.TryRemove(username.ToLower(), out Client);
        }

        public void CloseAll()
        {
            foreach (GameClient client in this.GetClients.ToList())
            {
                if (client == null)
                    continue;

                if (client.GetHabbo() != null)
                {
                    try
                    {
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunQuery(client.GetHabbo().GetQueryString);
                        }

                        Console.Clear();
                        log.Info("<<- SERVER SHUTDOWN ->> INVENTORY IS SAVING");
                    }
                    catch
                    {
                    }
                }
            }

            log.Info("Done saving users inventory!");
            log.Info("Closing server connections...");
            try
            {
                foreach (GameClient client in this.GetClients.ToList())
                {
                    if (client == null)
                        continue;

                    try
                    {
                        client.Dispose();
                    }
                    catch
                    {
                    }
                }
                Console.Clear();
                log.Info("<<- SERVER SHUTDOWN ->> CLOSING CONNECTIONS");
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
            }

            if (this._clients.Count > 0)
                this._clients.Clear();

            log.Info("Connections closed!");
        }

        private void TestClientConnections()
        {
            if (clientPingStopwatch.ElapsedMilliseconds >= 30000)
            {
                clientPingStopwatch.Restart();

                List<GameClient> ToPing = new List<GameClient>();

                foreach (GameClient client in this._clients.Values.ToList())
                {
                    if (client.PingCount < 6)
                    {
                        client.PingCount++;

                        ToPing.Add(client);
                    }
                    else
                    {
                        lock (timedOutConnections.SyncRoot)
                        {
                            timedOutConnections.Enqueue(client);
                        }
                    }
                }

                DateTime start = DateTime.Now;

                foreach (GameClient Client in ToPing.ToList())
                {
                    try
                    {
                        Client.SendPacket(new PongComposer());
                    }
                    catch
                    {
                        lock (timedOutConnections.SyncRoot)
                        {
                            timedOutConnections.Enqueue(Client);
                        }
                    }
                }
            }
        }

        private void HandleTimeouts()
        {
            if (timedOutConnections.Count > 0)
            {
                lock (timedOutConnections.SyncRoot)
                {
                    while (timedOutConnections.Count > 0)
                    {
                        GameClient client = null;

                        if (timedOutConnections.Count > 0)
                            client = (GameClient) timedOutConnections.Dequeue();

                        if (client != null)
                            client.Disconnect();
                    }
                }
            }
        }

        public int Count
        {
            get { return this._clients.Count; }
        }

        public ICollection<GameClient> GetClients
        {
            get { return this._clients.Values; }
        }
    }
}