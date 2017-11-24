using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms;


namespace Plus.HabboHotel.Rooms.Instance
{
    public class FilterComponent
    {
        private Room _instance = null;

        public FilterComponent(Room Instance)
        {
            if (Instance == null)
                return;

            this._instance = Instance;
        }

        public bool AddFilter(string Word)
        {
            if (this._instance.WordFilterList.Contains(Word))
                return false;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `room_filter` (`room_id`,`word`) VALUES(@rid,@word);");
                dbClient.AddParameter("rid", this._instance.Id);
                dbClient.AddParameter("word", Word);
                dbClient.RunQuery();
            }

            this._instance.WordFilterList.Add(Word);
            return true;
        }

        public bool RemoveFilter(string Word)
        {
            if (!this._instance.WordFilterList.Contains(Word))
                return false;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM `room_filter` WHERE `room_id` = @rid AND `word` = @word;");
                dbClient.AddParameter("rid", this._instance.Id);
                dbClient.AddParameter("word", Word);
                dbClient.RunQuery();
            }

            this._instance.WordFilterList.Remove(Word);
            return true;
        }

        public string CheckMessage(string Message)
        {
            foreach (string Filter in this._instance.WordFilterList)
            {
                if (Message.ToLower().Contains(Filter) || Message == Filter)
                    Message = Regex.Replace(Message, Filter, "Bobba", RegexOptions.IgnoreCase);
                else
                    continue;
            }

            return Message.TrimEnd(' ');
        }

        public void Cleanup()
        {
            this._instance = null;
        }
    }
}
