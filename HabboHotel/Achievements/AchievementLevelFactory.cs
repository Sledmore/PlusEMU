using System;
using System.Collections.Generic;
using System.Data;
using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Achievements
{
    public class AchievementLevelFactory
    {
        public static void GetAchievementLevels(out Dictionary<string, Achievement> Achievements)
        {
            Achievements = new Dictionary<string, Achievement>();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`category`,`group_name`,`level`,`reward_pixels`,`reward_points`,`progress_needed`,`game_id` FROM `achievements`");
                DataTable dTable = dbClient.GetTable();

                if (dTable != null)
                {
                    foreach (DataRow dRow in dTable.Rows)
                    {
                        int id = Convert.ToInt32(dRow["id"]);
                        string category = Convert.ToString(dRow["category"]);
                        string groupName = Convert.ToString(dRow["group_name"]);
                        int level = Convert.ToInt32(dRow["level"]);
                        int rewardPixels = Convert.ToInt32(dRow["reward_pixels"]);
                        int rewardPoints = Convert.ToInt32(dRow["reward_points"]);
                        int progressNeeded = Convert.ToInt32(dRow["progress_needed"]);

                        AchievementLevel AchievementLevel = new AchievementLevel(level, rewardPixels, rewardPoints, progressNeeded);

                        if (!Achievements.ContainsKey(groupName))
                        {
                            Achievement Achievement = new Achievement(id, groupName, category, Convert.ToInt32(dRow["game_id"]));
                            Achievement.AddLevel(AchievementLevel);
                            Achievements.Add(groupName, Achievement);
                        }
                        else
                            Achievements[groupName].AddLevel(AchievementLevel);
                    }
                }
            }
        }
    }
}