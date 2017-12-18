using System;
using System.Collections.Generic;
using System.Data;
using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Achievements
{
    public class AchievementLevelFactory
    {
        public static void GetAchievementLevels(out Dictionary<string, Achievement> achievements)
        {
            achievements = new Dictionary<string, Achievement>();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`category`,`group_name`,`level`,`reward_pixels`,`reward_points`,`progress_needed`,`game_id` FROM `achievements`");
                DataTable table = dbClient.GetTable();

                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        int id = Convert.ToInt32(row["id"]);
                        string category = Convert.ToString(row["category"]);
                        string groupName = Convert.ToString(row["group_name"]);
                        int level = Convert.ToInt32(row["level"]);
                        int rewardPixels = Convert.ToInt32(row["reward_pixels"]);
                        int rewardPoints = Convert.ToInt32(row["reward_points"]);
                        int progressNeeded = Convert.ToInt32(row["progress_needed"]);

                        AchievementLevel AchievementLevel = new AchievementLevel(level, rewardPixels, rewardPoints, progressNeeded);

                        if (!achievements.ContainsKey(groupName))
                        {
                            Achievement Achievement = new Achievement(id, groupName, category, Convert.ToInt32(row["game_id"]));
                            Achievement.AddLevel(AchievementLevel);
                            achievements.Add(groupName, Achievement);
                        }
                        else
                            achievements[groupName].AddLevel(AchievementLevel);
                    }
                }
            }
        }
    }
}