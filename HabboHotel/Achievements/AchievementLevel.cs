namespace Plus.HabboHotel.Achievements
{
    public struct AchievementLevel
    {
        public int Level { get; private set; }
        public int Requirement { get; private set; }
        public int RewardPixels { get; private set; }
        public int RewardPoints { get; private set; }

        public AchievementLevel(int level, int rewardPixels, int rewardPoints, int requirement)
        {
            Level = level;
            RewardPixels = rewardPixels;
            RewardPoints = rewardPoints;
            Requirement = requirement;
        }
    }
}