﻿using System;
using System.Threading;
using Plus.Database.Interfaces;
using Serilog;

namespace Plus.Core
{
    public class ServerStatusUpdater : IDisposable
    {
        private const int UpdateInSeconds = 30;

        private Timer _timer;

        public void Init()
        {
            _timer = new Timer(OnTick, null, TimeSpan.FromSeconds(UpdateInSeconds), TimeSpan.FromSeconds(UpdateInSeconds));

            Console.Title = "Plus Emulator - 0 users online - 0 rooms loaded - 0 day(s) 0 hour(s) uptime";

            Log.Information("Server Status Updater has been started.");
        }

        public void OnTick(object obj)
        {
            UpdateOnlineUsers();
        }

        private void UpdateOnlineUsers()
        {
            TimeSpan uptime = DateTime.Now - PlusEnvironment.ServerStarted;

            int usersOnline = PlusEnvironment.GetGame().GetClientManager().Count;
            int roomCount = PlusEnvironment.GetGame().GetRoomManager().Count;

            Console.Title = "Plus Emulator - " + usersOnline + " users online - " + roomCount + " rooms loaded - " + uptime.Days + " day(s) " + uptime.Hours + " hour(s) uptime";

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `server_status` SET `users_online` = @users, `loaded_rooms` = @loadedRooms LIMIT 1;");
                dbClient.AddParameter("users", usersOnline);
                dbClient.AddParameter("loadedRooms", roomCount);
                dbClient.RunQuery();
            }
        }

        public void Dispose()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `server_status` SET `users_online` = '0', `loaded_rooms` = '0'");
            }

            _timer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
