using Plus.HabboHotel.Achievements;

namespace Plus.Communication.Packets.Outgoing.Inventory.Achievements
{
    class AchievementProgressedComposer : ServerPacket
    {
        public AchievementProgressedComposer(Achievement Achievement, int TargetLevel, AchievementLevel TargetLevelData,int TotalLevels, UserAchievement UserData)
            : base(ServerPacketHeader.AchievementProgressedMessageComposer)
        {
            WriteInteger(Achievement.Id); // Unknown (ID?)
            WriteInteger(TargetLevel); // Target level
           WriteString(Achievement.GroupName + TargetLevel); // Target name/desc/badge
            WriteInteger(1); // Progress req/target 
            WriteInteger(TargetLevelData.Requirement); // Reward in Pixels
            WriteInteger(TargetLevelData.RewardPixels); // Reward Ach Score
            WriteInteger(0); // ?
            WriteInteger(UserData != null ? UserData.Progress : 0); // Current progress
            WriteBoolean(UserData != null ? (UserData.Level >= TotalLevels) : false); // Set 100% completed(??)
           WriteString(Achievement.Category); // Category
           WriteString(string.Empty);
            WriteInteger(TotalLevels); // Total amount of levels 
            WriteInteger(0);
        }
    }
}
