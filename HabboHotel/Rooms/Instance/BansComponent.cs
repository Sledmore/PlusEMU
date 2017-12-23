using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Plus.Utilities;
using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Rooms.Instance
{
    public class BansComponent
    {
        /// <summary>
        /// The RoomInstance that created this BanComponent.
        /// </summary>
        private Room _instance = null;

        /// <summary>
        /// The bans collection for storing them for this room.
        /// </summary>
        private ConcurrentDictionary<int, double> _bans;

        /// <summary>
        /// Create the BanComponent for the RoomInstance.
        /// </summary>
        /// <param name="instance">The instance that created this component.</param>
        public BansComponent(Room Instance)
        {
            if (Instance == null)
                return;

            this._instance = Instance;
            this._bans = new ConcurrentDictionary<int, double>();

            DataTable GetBans = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `user_id`, `expire` FROM `room_bans` WHERE `room_id` = " + this._instance.Id + " AND `expire` > UNIX_TIMESTAMP();");
                GetBans = dbClient.GetTable();

                if (GetBans != null)
                {
                    foreach (DataRow Row in GetBans.Rows)
                    {
                        this._bans.TryAdd(Convert.ToInt32(Row["user_id"]), Convert.ToDouble(Row["expire"]));
                    }
                }
            }
        }

        public void Ban(RoomUser Avatar, double Time)
        {
            if (Avatar == null || this._instance.CheckRights(Avatar.GetClient(), true) || this.IsBanned(Avatar.UserId))
                return;

            double BanTime = UnixTimestamp.GetNow() + Time;
            if (!this._bans.TryAdd(Avatar.UserId, BanTime))
                this._bans[Avatar.UserId] = BanTime;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("REPLACE INTO `room_bans` (`user_id`,`room_id`,`expire`) VALUES (@uid, @rid, @expire);");
                dbClient.AddParameter("rid", this._instance.Id);
                dbClient.AddParameter("uid", Avatar.UserId);
                dbClient.AddParameter("expire", BanTime);
                dbClient.RunQuery();
            }

            this._instance.GetRoomUserManager().RemoveUserFromRoom(Avatar.GetClient(), true, true);
        }

        public bool IsBanned(int UserId)
        {
            if (!this._bans.ContainsKey(UserId))
                return false;

            double BanTime = this._bans[UserId] - UnixTimestamp.GetNow();
            if (BanTime <= 0)
            {
                double time;
                this._bans.TryRemove(UserId, out time);

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("DELETE FROM `room_bans` WHERE `room_id` = @rid AND `user_id` = @uid;");
                    dbClient.AddParameter("rid", this._instance.Id);
                    dbClient.AddParameter("uid", UserId);
                    dbClient.RunQuery();
                }
                return false;
            }

            return true;
        }

        public bool Unban(int UserId)
        {
            if (!this._bans.ContainsKey(UserId))
                return false;

            if (this._bans.TryRemove(UserId, out double time))
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("DELETE FROM `room_bans` WHERE `room_id` = @rid AND `user_id` = @uid;");
                    dbClient.AddParameter("rid", this._instance.Id);
                    dbClient.AddParameter("uid", UserId);
                    dbClient.RunQuery();
                }
                return true;
            }

            return false;
        }

        public List<int> BannedUsers()
        {
            DataTable GetBans = null;
            List<int> Bans = new List<int>();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `user_id` FROM `room_bans` WHERE `room_id` = '" + this._instance.Id + "' AND `expire` > UNIX_TIMESTAMP();");
                GetBans = dbClient.GetTable();

                if (GetBans != null)
                {
                    foreach (DataRow Row in GetBans.Rows)
                    {
                        if (!Bans.Contains(Convert.ToInt32(Row["user_id"])))
                            Bans.Add(Convert.ToInt32(Row["user_id"]));
                    }
                }
            }

            return Bans;
        }

        public int Count
        {
            get { return this._bans.Count; }
        }

        public void Cleanup()
        {
            this._bans.Clear();

            this._instance = null;
            this._bans = null;
        }
    }
}