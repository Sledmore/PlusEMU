using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

using log4net;
using Plus.Database.Interfaces;

using Plus.HabboHotel.Achievements;

namespace Plus.HabboHotel.Talents
{
    public class TalentTrackManager
    {
        private static ILog log = LogManager.GetLogger("Plus.HabboHotel.Talents.TalentManager");

        private readonly Dictionary<int, TalentTrackLevel> _citizenshipLevels;

        public TalentTrackManager()
        {
            _citizenshipLevels = new Dictionary<int, TalentTrackLevel>();
        }

        public void Init()
        {
            DataTable data = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `type`,`level`,`data_actions`,`data_gifts` FROM `talents`");
                data = dbClient.GetTable();
            }

            if (data != null)
            {
                foreach (DataRow row in data.Rows)
                {
                    _citizenshipLevels.Add(Convert.ToInt32(row["level"]), new TalentTrackLevel(Convert.ToString(row["type"]), Convert.ToInt32(row["level"]), Convert.ToString(row["data_actions"]), Convert.ToString(row["data_gifts"])));
                }
            }

            log.Info("Loaded " + _citizenshipLevels.Count + " talent track levels");
        }

        public ICollection<TalentTrackLevel> GetLevels()
        {
            return _citizenshipLevels.Values;
        }
    }
}
