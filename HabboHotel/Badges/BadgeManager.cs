using System;
using System.Data;
using System.Collections.Generic;
using Plus.Database.Interfaces;
using Serilog;

namespace Plus.HabboHotel.Badges
{
    public class BadgeManager
    {

        private readonly Dictionary<string, BadgeDefinition> _badges;

        public BadgeManager()
        {
            _badges = new Dictionary<string, BadgeDefinition>();
        }

        public void Init()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `badge_definitions`;");
                DataTable data = dbClient.GetTable();

                foreach (DataRow row in data.Rows)
                {
                    string code = Convert.ToString(row["code"]).ToUpper();

                    if (!_badges.ContainsKey(code))
                        _badges.Add(code, new BadgeDefinition(code, Convert.ToString(row["required_right"])));
                }
            }

            Log.Information("Loaded " + _badges.Count + " badge definitions.");
        }
   
        public bool TryGetBadge(string code, out BadgeDefinition badge)
        {
            return _badges.TryGetValue(code.ToUpper(), out badge);
        }
    }
}