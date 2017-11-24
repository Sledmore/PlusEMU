namespace Plus.HabboHotel.Achievements
{
    public class UserAchievement
    {
        public readonly string AchievementGroup;
        public int Level;
        public int Progress;

        public UserAchievement(string achievementGroup, int level, int progress)
        {
            this.AchievementGroup = achievementGroup;
            this.Level = level;
            this.Progress = progress;
        }
    }
}