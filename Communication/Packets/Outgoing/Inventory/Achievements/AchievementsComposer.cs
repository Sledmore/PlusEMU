using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Achievements;

namespace Plus.Communication.Packets.Outgoing.Inventory.Achievements
{
    class AchievementsComposer : ServerPacket
    {
        public AchievementsComposer(GameClient Session, List<Achievement> Achievements)
            : base(ServerPacketHeader.AchievementsMessageComposer)
        {
            base.WriteInteger(Achievements.Count);
            foreach (Achievement Achievement in Achievements)
            {
                UserAchievement UserData = Session.GetHabbo().GetAchievementData(Achievement.GroupName);
                int TargetLevel = (UserData != null ? UserData.Level + 1 : 1);
                int TotalLevels = Achievement.Levels.Count;

                TargetLevel = (TargetLevel > TotalLevels ? TotalLevels : TargetLevel);

                AchievementLevel TargetLevelData = Achievement.Levels[TargetLevel];
                base.WriteInteger(Achievement.Id); // Unknown (ID?)
                base.WriteInteger(TargetLevel); // Target level
               base.WriteString(Achievement.GroupName + TargetLevel); // Target name/desc/badge

                base.WriteInteger(1);
                base.WriteInteger(TargetLevelData.Requirement); // Progress req/target          
                base.WriteInteger(TargetLevelData.RewardPixels);

                base.WriteInteger(0); // Type of reward
                base.WriteInteger(UserData != null ? UserData.Progress : 0); // Current progress
                
                base.WriteBoolean(UserData != null ? (UserData.Level >= TotalLevels) : false);// Set 100% completed(??)
               base.WriteString(Achievement.Category); // Category
               base.WriteString(string.Empty);
                base.WriteInteger(TotalLevels); // Total amount of levels 
                base.WriteInteger(0);
            }
           base.WriteString("");
        }
    }
}