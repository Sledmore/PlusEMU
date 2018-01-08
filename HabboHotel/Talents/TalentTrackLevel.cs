using System;
using System.Data;
using System.Collections.Generic;

using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Talents
{
    public class TalentTrackLevel
    {
        public string Type { get; set; }
        public int Level { get; set; }

        private List<string> _dataActions;
        private List<string> _dataGifts;

        private Dictionary<int, TalentTrackSubLevel> _subLevels;

        public TalentTrackLevel(string Type, int Level, string DataActions, string DataGifts)
        {
            this.Type = Type;
            this.Level = Level;

            foreach (string Str in DataActions.Split('|'))
            {
                if (this._dataActions == null) { this._dataActions = new List<string>(); }
                this._dataActions.Add(Str);
            }

            foreach (string Str in DataGifts.Split('|'))
            {
                if (this._dataGifts == null) { this._dataGifts = new List<string>(); }
                this._dataGifts.Add(Str);
            }

            this._subLevels = new Dictionary<int, TalentTrackSubLevel>();

            this.Init();
        }

        public List<string> Actions
        {
            get { return this._dataActions; }
            private set { this._dataActions = value; }
        }

        public List<string> Gifts
        {
            get { return this._dataGifts; }
            private set { this._dataGifts = value; }
        }

        public void Init()
        {
            DataTable GetTable = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `sub_level`,`badge_code`,`required_progress` FROM `talents_sub_levels` WHERE `talent_level` = @TalentLevel");
                dbClient.AddParameter("TalentLevel", this.Level);
                GetTable = dbClient.GetTable();
            }

            if (GetTable != null)
            {
                foreach (DataRow Row in GetTable.Rows)
                {
                    this._subLevels.Add(Convert.ToInt32(Row["sub_level"]), new TalentTrackSubLevel(Convert.ToInt32(Row["sub_level"]), Convert.ToString(Row["badge_code"]), Convert.ToInt32(Row["required_progress"])));
                }
            }
        }

        public ICollection<TalentTrackSubLevel> GetSubLevels()
        {
            return this._subLevels.Values;
        }
    }
}
