using System;
using System.Data;
using System.Collections.Generic;

using log4net;
using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Badges
{
    public class BadgeManager
    {
        private static readonly ILog log = LogManager.GetLogger("Plus.HabboHotel.Badges.BadgeManager");

        private readonly Dictionary<string, BadgeDefinition> _badges;

        public BadgeManager()
        {
            this._badges = new Dictionary<string, BadgeDefinition>();
        }

        public void Init()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `badge_definitions`;");
                DataTable GetBadges = dbClient.GetTable();

                foreach (DataRow Row in GetBadges.Rows)
                {
                    string BadgeCode = Convert.ToString(Row["code"]).ToUpper();

                    if (!this._badges.ContainsKey(BadgeCode))
                        this._badges.Add(BadgeCode, new BadgeDefinition(BadgeCode, Convert.ToString(Row["required_right"])));
                }
            }

            log.Info("Loaded " + this._badges.Count + " badge definitions.");
        }
   
        public bool TryGetBadge(string BadgeCode, out BadgeDefinition Badge)
        {
            return this._badges.TryGetValue(BadgeCode.ToUpper(), out Badge);
        }
    }
}