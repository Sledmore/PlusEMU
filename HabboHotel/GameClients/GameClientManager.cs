﻿using System;
using System.Collections.Generic;
using System.Text;
using Plus.Core;
using Plus.HabboHotel.Users.Messenger;
using System.Linq;
using System.Collections.Concurrent;
using Plus.Communication.Packets.Outgoing;
using System.Data;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.Database.Interfaces;
using System.Collections;
using Plus.Communication.Packets.Outgoing.Handshake;
using System.Diagnostics;
using Plus.Communication.ConnectionManager;
using Serilog;

namespace Plus.HabboHotel.GameClients
{
    public class GameClientManager
    {
        private readonly ConcurrentDictionary<int, GameClient> _clients;
        private readonly ConcurrentDictionary<int, GameClient> _userIdRegister;
        private readonly ConcurrentDictionary<string, GameClient> _usernameRegister;

        private readonly Queue _timedOutConnections;

        private readonly Stopwatch _clientPingStopwatch;

        public GameClientManager()
        {
            _clients = new ConcurrentDictionary<int, GameClient>();
            _userIdRegister = new ConcurrentDictionary<int, GameClient>();
            _usernameRegister = new ConcurrentDictionary<string, GameClient>();

            _timedOutConnections = new Queue();

            _clientPingStopwatch = new Stopwatch();
            _clientPingStopwatch.Start();
        }

        public void OnCycle()
        {
            TestClientConnections();
            HandleTimeouts();
        }

        public GameClient GetClientByUserId(int userId)
        {
            return _userIdRegister.ContainsKey(userId) ? _userIdRegister[userId] : null;
        }

        public GameClient GetClientByUsername(string username)
        {
            return _usernameRegister.ContainsKey(username.ToLower()) ? _usernameRegister[username.ToLower()] : null;
        }

        public bool TryGetClient(int clientId, out GameClient client)
        {
            return _clients.TryGetValue(clientId, out client);
        }

        public bool UpdateClientUsername(GameClient client, string oldUsername, string newUsername)
        {
            if (client == null || !_usernameRegister.ContainsKey(oldUsername.ToLower()))
                return false;

            _usernameRegister.TryRemove(oldUsername.ToLower(), out client);
            _usernameRegister.TryAdd(newUsername.ToLower(), client);
            return true;
        }

        public string GetNameById(int id)
        {
            GameClient client = GetClientByUserId(id);

            if (client != null)
                return client.GetHabbo().Username;

            string username;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT username FROM users WHERE id = @id LIMIT 1");
                dbClient.AddParameter("id", id);
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

        public void StaffAlert(ServerPacket message, int exclude = 0)
        {
            foreach (GameClient client in GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                    continue;

                if (client.GetHabbo().Rank < 2 || client.GetHabbo().Id == exclude)
                    continue;

                client.SendPacket(message);
            }
        }

        public void ModAlert(string message)
        {
            foreach (GameClient client in GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                    continue;

                if (client.GetHabbo().GetPermissions().HasRight("mod_tool") && !client.GetHabbo().GetPermissions().HasRight("staff_ignore_mod_alert"))
                {
                    try { client.SendWhisper(message, 5); }
                    catch { }
                }
            }
        }

        public void DoAdvertisingReport(GameClient reporter, GameClient target)
        {
            if (reporter == null || target == null || reporter.GetHabbo() == null || target.GetHabbo() == null)
                return;

            StringBuilder builder = new StringBuilder();
            builder.Append("New report submitted!\r\r");
            builder.Append("Reporter: " + reporter.GetHabbo().Username + "\r");
            builder.Append("Reported User: " + target.GetHabbo().Username + "\r\r");
            builder.Append(target.GetHabbo().Username + "s last 10 messages:\r\r");

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `message` FROM `chatlogs` WHERE `user_id` = '" + target.GetHabbo().Id + "' ORDER BY `id` DESC LIMIT 10");
                DataTable logs = dbClient.GetTable();

                if (logs != null)
                {
                    int number = 11;
                    foreach (DataRow log in logs.Rows)
                    {
                        number -= 1;
                        builder.Append(number + ": " + Convert.ToString(log["message"]) + "\r");
                    }
                }
            }

            foreach (GameClient client in GetClients.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                    continue;

                if (client.GetHabbo().GetPermissions().HasRight("mod_tool") && !client.GetHabbo().GetPermissions().HasRight("staff_ignore_advertisement_reports"))
                    client.SendPacket(new MotdNotificationComposer(builder.ToString()));
            }
        }


        public void SendPacket(ServerPacket packet, string fuse = "")
        {
            foreach (GameClient client in _clients.Values.ToList())
            {
                if (client == null || client.GetHabbo() == null)
                    continue;

                if (!string.IsNullOrEmpty(fuse))
                {
                    if (!client.GetHabbo().GetPermissions().HasRight(fuse))
                        continue;
                }

                client.SendPacket(packet);
            }
        }

        public void CreateAndStartClient(int clientId, ConnectionInformation connection)
        {
            GameClient client = new GameClient(clientId, connection);
            if (_clients.TryAdd(client.ConnectionId, client))
                client.StartConnection();
            else
                connection.Dispose();
        }

        public void DisposeConnection(int clientId)
        {
            if (!TryGetClient(clientId, out GameClient client))
                return;

            if (client != null)
                client.Dispose();

            _clients.TryRemove(clientId, out client);
        }

        public void LogClonesOut(int userId)
        {
            GameClient client = GetClientByUserId(userId);
            if (client != null)
                client.Disconnect();
        }

        public void RegisterClient(GameClient client, int userId, string username)
        {
            if (_usernameRegister.ContainsKey(username.ToLower()))
                _usernameRegister[username.ToLower()] = client;
            else
                _usernameRegister.TryAdd(username.ToLower(), client);

            if (_userIdRegister.ContainsKey(userId))
                _userIdRegister[userId] = client;
            else
                _userIdRegister.TryAdd(userId, client);
        }

        public void UnregisterClient(int userid, string username)
        {
            _userIdRegister.TryRemove(userid, out _);
            _usernameRegister.TryRemove(username.ToLower(), out _);
        }

        public void CloseAll()
        {
            foreach (GameClient client in GetClients.ToList())
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
                        Log.Information("<<- SERVER SHUTDOWN ->> IVNENTORY IS SAVING");
                    }
                    catch
                    {
                    }
                }
            }

            Log.Information("Done saving users inventory!");
            Log.Information("Closing server connections...");
            try
            {
                foreach (GameClient client in GetClients.ToList())
                {
                    if (client == null || client.GetConnection() == null)
                        continue;

                    try
                    {
                        client.GetConnection().Dispose();
                    }
                    catch { }

                    Console.Clear();
                    Log.Information("<<- SERVER SHUTDOWN ->> CLOSING CONNECTIONS");

                }
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
            }

            if (_clients.Count > 0)
                _clients.Clear();

            Log.Information("Connections closed!");
        }

        private void TestClientConnections()
        {
            if (_clientPingStopwatch.ElapsedMilliseconds >= 30000)
            {
                _clientPingStopwatch.Restart();

                try
                {
                    List<GameClient> toPing = new List<GameClient>();

                    foreach (GameClient client in _clients.Values.ToList())
                    {
                        if (client.PingCount < 6)
                        {
                            client.PingCount++;

                            toPing.Add(client);
                        }
                        else
                        {
                            lock (_timedOutConnections.SyncRoot)
                            {
                                _timedOutConnections.Enqueue(client);
                            }
                        }
                    }

                    DateTime start = DateTime.Now;

                    foreach (GameClient client in toPing.ToList())
                    {
                        try
                        {
                            client.SendPacket(new PongComposer());
                        }
                        catch
                        {
                            lock (_timedOutConnections.SyncRoot)
                            {
                                _timedOutConnections.Enqueue(client);
                            }
                        }
                    }

                }
                catch (Exception)
                {
                    //ignored
                }
            }
        }

        private void HandleTimeouts()
        {
            if (_timedOutConnections.Count > 0)
            {
                lock (_timedOutConnections.SyncRoot)
                {
                    while (_timedOutConnections.Count > 0)
                    {
                        GameClient client = null;

                        if (_timedOutConnections.Count > 0)
                            client = (GameClient)_timedOutConnections.Dequeue();

                        if (client != null)
                            client.Disconnect();
                    }
                }
            }
        }

        public int Count
        {
            get { return _clients.Count; }
        }

        public ICollection<GameClient> GetClients
        {
            get
            {
                return _clients.Values;
            }
        }
    }
}