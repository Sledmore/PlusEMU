using System.Collections.Generic;

namespace Plus.HabboHotel.Achievements
{
    public class Achievement
    {
        public int Id { get; private set; }
        public string Category { get; private set; }
        public string GroupName { get; private set; }
        public int GameId { get; private set; }

        public Dictionary<int, AchievementLevel> Levels;

        public Achievement(int id, string groupName, string category, int gameId)
        {
            Id = id;
            GroupName = groupName;
            Category = category;
            GameId = gameId;
            Levels = new Dictionary<int, AchievementLevel>();
        }

        public void AddLevel(AchievementLevel level)
        {
            Levels.Add(level.Level, level);
        }
    }
}