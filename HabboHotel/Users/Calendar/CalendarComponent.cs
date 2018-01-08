using System;
using System.Data;
using System.Collections.Generic;

using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Users.Calendar
{
    /// <summary>
    /// Permissions for a specific Player.
    /// </summary>
    public sealed class CalendarComponent
    {
        /// <summary>
        /// Permission rights are stored here.
        /// </summary>
        private readonly List<int> _lateBoxes;

        private readonly List<int> _openedBoxes;

        public CalendarComponent()
        {
            this._lateBoxes = new List<int>();
            this._openedBoxes = new List<int>();
        }

        /// <summary>
        /// Initialize the PermissionComponent.
        /// </summary>
        /// <param name="Player"></param>
        public bool Init(Habbo Player)
        {
            if (this._lateBoxes.Count > 0)
                this._lateBoxes.Clear();

            if (this._openedBoxes.Count > 0)
                this._openedBoxes.Clear();

            DataTable GetData = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `user_xmas15_calendar` WHERE `user_id` = @id;");
                dbClient.AddParameter("id", Player.Id);
                GetData = dbClient.GetTable();

                if (GetData != null)
                {
                    foreach (DataRow Row in GetData.Rows)
                    {
                        if (Convert.ToInt32(Row["status"]) == 0)
                        {
                            this._lateBoxes.Add(Convert.ToInt32(Row["day"]));
                        }
                        else
                        {
                            this._openedBoxes.Add(Convert.ToInt32(Row["day"]));
                        }
                    }
                }
            }
            return true;
        }

        public List<int> GetOpenedBoxes()
        {
            return this._openedBoxes;
        }

        public List<int> GetLateBoxes()
        {
            return this._lateBoxes;
        }

        /// <summary>
        /// Dispose of the permissions list.
        /// </summary>
        public void Dispose()
        {
            this._lateBoxes.Clear();
            this._openedBoxes.Clear();
        }
    }
}
