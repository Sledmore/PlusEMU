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
            WriteInteger(Achievements.Count);
            foreach (Achievement Achievement in Achievements)
            {
                UserAchievement UserData = Session.GetHabbo().GetAchievementData(Achievement.GroupName);
                int TargetLevel = (UserData != null ? UserData.Level + 1 : 1);
                int TotalLevels = Achievement.Levels.Count;

                TargetLevel = (TargetLevel > TotalLevels ? TotalLevels : TargetLevel);

                AchievementLevel TargetLevelData = Achievement.Levels[TargetLevel];
                WriteInteger(Achievement.Id); // Unknown (ID?)
                WriteInteger(TargetLevel); // Target level
               WriteString(Achievement.GroupName + TargetLevel); // Target name/desc/badge

                WriteInteger(1);
                WriteInteger(TargetLevelData.Requirement); // Progress req/target          
                WriteInteger(TargetLevelData.RewardPixels);

                WriteInteger(0); // Type of reward
                WriteInteger(UserData != null ? UserData.Progress : 0); // Current progress
                
                WriteBoolean(UserData != null ? (UserData.Level >= TotalLevels) : false);// Set 100% completed(??)
               WriteString(Achievement.Category); // Category
               WriteString(string.Empty);
                WriteInteger(TotalLevels); // Total amount of levels 
                WriteInteger(0);
            }
           WriteString("");
        }
    }
}