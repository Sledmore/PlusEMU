using System;
using System.Data;
using System.Collections.Generic;

using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Users.Ignores
{
    public sealed class IgnoresComponent
    {
        private readonly List<int> _ignoredUsers;

        public IgnoresComponent()
        {
            this._ignoredUsers = new List<int>();
        }

        public bool Init(Habbo player)
        {
            if (this._ignoredUsers.Count > 0)
                return false;
            
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `user_ignores` WHERE `user_id` = @uid;");
                dbClient.AddParameter("uid", player.Id);
                DataTable GetIgnores = dbClient.GetTable();

                if (GetIgnores != null)
                {
                    foreach (DataRow Row in GetIgnores.Rows)
                    {
                        this._ignoredUsers.Add(Convert.ToInt32(Row["ignore_id"]));
                    }
                }
            }
            return true;
        }

        public bool TryGet(int userId)
        {
            return this._ignoredUsers.Contains(userId);
        }

        public bool TryAdd(int userId)
        {
            if (this._ignoredUsers.Contains(userId))
                return false;

            this._ignoredUsers.Add(userId);
            return true;
        }

        public bool TryRemove(int userId)
        {
            return this._ignoredUsers.Remove(userId);
        }

        public ICollection<int> IgnoredUserIds()
        {
           return this._ignoredUsers;
        }

        public void Dispose()
        {
            this._ignoredUsers.Clear();
        }
    }
}