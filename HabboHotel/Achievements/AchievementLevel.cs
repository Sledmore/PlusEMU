namespace Plus.HabboHotel.Achievements
{
    public struct AchievementLevel
    {
        public int Level { get; }
        public int Requirement { get; }
        public int RewardPixels { get; }
        public int RewardPoints { get; }

        public AchievementLevel(int level, int rewardPixels, int rewardPoints, int requirement)
        {
            Level = level;
            RewardPixels = rewardPixels;
            RewardPoints = rewardPoints;
            Requirement = requirement;
        }
    }
}