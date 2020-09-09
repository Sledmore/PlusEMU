using Plus.HabboHotel.Achievements;

namespace Plus.Communication.Packets.Outgoing.Inventory.Achievements
{
    class AchievementProgressedComposer : MessageComposer
    {
        public Achievement Achievement { get; }
        public int TargetLevel { get; }
        public AchievementLevel TargetLevelData { get; }
        public int TotalLevels { get; }
        public UserAchievement UserData { get; }

        public AchievementProgressedComposer(Achievement Achievement, int TargetLevel, AchievementLevel TargetLevelData,int TotalLevels, UserAchievement UserData)
            : base(ServerPacketHeader.AchievementProgressedMessageComposer)
        {
            this.Achievement = Achievement;
            this.TargetLevel = TargetLevel;
            this.TargetLevelData = TargetLevelData;
            this.TotalLevels = TotalLevels;
            this.UserData = UserData;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Achievement.Id); // Unknown (ID?)
            packet.WriteInteger(TargetLevel); // Target level
            packet.WriteString(Achievement.GroupName + TargetLevel); // Target name/desc/badge
            packet.WriteInteger(1); // Progress req/target 
            packet.WriteInteger(TargetLevelData.Requirement); // Reward in Pixels
            packet.WriteInteger(TargetLevelData.RewardPixels); // Reward Ach Score
            packet.WriteInteger(0); // ?
            packet.WriteInteger(UserData != null ? UserData.Progress : 0); // Current progress
            packet.WriteBoolean(UserData != null ? (UserData.Level >= TotalLevels) : false); // Set 100% completed(??)
            packet.WriteString(Achievement.Category); // Category
            packet.WriteString(string.Empty);
            packet.WriteInteger(TotalLevels); // Total amount of levels 
            packet.WriteInteger(0);
        }
    }
}
