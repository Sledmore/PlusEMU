using System;
using System.Threading;

using log4net;
using Plus.Database.Interfaces;

namespace Plus.Core
{
    public class ServerStatusUpdater : IDisposable
    {
        private static ILog log = LogManager.GetLogger("Plus.Core.ServerUpdater");

        private const int UPDATE_IN_SECS = 30;

        private Timer _timer;
        
        public ServerStatusUpdater()
        {
        }

        public void Init()
        {
            this._timer = new Timer(new TimerCallback(this.OnTick), null, TimeSpan.FromSeconds(UPDATE_IN_SECS), TimeSpan.FromSeconds(UPDATE_IN_SECS));

            Console.Title = "Plus Emulator - 0 users online - 0 rooms loaded - 0 day(s) 0 hour(s) uptime";

            log.Info("Server Status Updater has been started.");
        }

        public void OnTick(object Obj)
        {
            this.UpdateOnlineUsers();
        }

        private void UpdateOnlineUsers()
        {
            TimeSpan Uptime = DateTime.Now - PlusEnvironment.ServerStarted;

            int UsersOnline = PlusEnvironment.GetGame().GetClientManager().Count;
            int RoomCount = PlusEnvironment.GetGame().GetRoomManager().Count;

            Console.Title = "Plus Emulator - " + UsersOnline + " users online - " + RoomCount + " rooms loaded - " + Uptime.Days + " day(s) " + Uptime.Hours + " hour(s) uptime";

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `server_status` SET `users_online` = @users, `loaded_rooms` = @loadedRooms LIMIT 1;");
                dbClient.AddParameter("users", UsersOnline);
                dbClient.AddParameter("loadedRooms", RoomCount);
                dbClient.RunQuery();
            }
        }

        public void Dispose()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `server_status` SET `users_online` = '0', `loaded_rooms` = '0'");
            }

            this._timer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
