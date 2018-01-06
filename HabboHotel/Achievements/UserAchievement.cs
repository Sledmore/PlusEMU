namespace Plus.HabboHotel.Achievements
{
    public class UserAchievement
    {
        public string AchievementGroup { get; }
        public int Level { get; set; }
        public int Progress { get; set; }

        public UserAchievement(string achievementGroup, int level, int progress)
        {
            AchievementGroup = achievementGroup;
            Level = level;
            Progress = progress;
        }
    }
}