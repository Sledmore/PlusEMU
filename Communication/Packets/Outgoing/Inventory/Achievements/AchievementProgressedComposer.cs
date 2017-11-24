using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Achievements;

namespace Plus.Communication.Packets.Outgoing.Inventory.Achievements
{
    class AchievementProgressedComposer : ServerPacket
    {
        public AchievementProgressedComposer(Achievement Achievement, int TargetLevel, AchievementLevel TargetLevelData,int TotalLevels, UserAchievement UserData)
            : base(ServerPacketHeader.AchievementProgressedMessageComposer)
        {
            base.WriteInteger(Achievement.Id); // Unknown (ID?)
            base.WriteInteger(TargetLevel); // Target level
           base.WriteString(Achievement.GroupName + TargetLevel); // Target name/desc/badge
            base.WriteInteger(1); // Progress req/target 
            base.WriteInteger(TargetLevelData.Requirement); // Reward in Pixels
            base.WriteInteger(TargetLevelData.RewardPixels); // Reward Ach Score
            base.WriteInteger(0); // ?
            base.WriteInteger(UserData != null ? UserData.Progress : 0); // Current progress
            base.WriteBoolean(UserData != null ? (UserData.Level >= TotalLevels) : false); // Set 100% completed(??)
           base.WriteString(Achievement.Category); // Category
           base.WriteString(string.Empty);
            base.WriteInteger(TotalLevels); // Total amount of levels 
            base.WriteInteger(0);
        }
    }
}
