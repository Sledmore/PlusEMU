namespace Plus.HabboHotel.Achievements
{
    public struct AchievementLevel
    {
        public readonly int Level;
        public readonly int Requirement;
        public readonly int RewardPixels;
        public readonly int RewardPoints;

        public AchievementLevel(int level, int rewardPixels, int rewardPoints, int requirement)
        {
            Level = level;
            RewardPixels = rewardPixels;
            RewardPoints = rewardPoints;
            Requirement = requirement;
        }
    }
}