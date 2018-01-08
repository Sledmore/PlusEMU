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
            _lateBoxes = new List<int>();
            _openedBoxes = new List<int>();
        }

        /// <summary>
        /// Initialize the PermissionComponent.
        /// </summary>
        /// <param name="Player"></param>
        public bool Init(Habbo Player)
        {
            if (_lateBoxes.Count > 0)
                _lateBoxes.Clear();

            if (_openedBoxes.Count > 0)
                _openedBoxes.Clear();

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
                            _lateBoxes.Add(Convert.ToInt32(Row["day"]));
                        }
                        else
                        {
                            _openedBoxes.Add(Convert.ToInt32(Row["day"]));
                        }
                    }
                }
            }
            return true;
        }

        public List<int> GetOpenedBoxes()
        {
            return _openedBoxes;
        }

        public List<int> GetLateBoxes()
        {
            return _lateBoxes;
        }

        /// <summary>
        /// Dispose of the permissions list.
        /// </summary>
        public void Dispose()
        {
            _lateBoxes.Clear();
            _openedBoxes.Clear();
        }
    }
}
