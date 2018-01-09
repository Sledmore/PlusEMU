using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Achievements;

namespace Plus.Communication.Packets.Outgoing.GameCenter
{
    class GameAchievementListComposer : ServerPacket
    {
        public GameAchievementListComposer(GameClient Session, ICollection<Achievement> Achievements, int GameId)
            : base(ServerPacketHeader.GameAchievementListMessageComposer)
        {
            WriteInteger(GameId);
            WriteInteger(Achievements.Count);
            foreach (Achievement Ach in Achievements.ToList())
            {
                UserAchievement UserData = Session.GetHabbo().GetAchievementData(Ach.GroupName);
                int TargetLevel = (UserData != null ? UserData.Level + 1 : 1);
           
                AchievementLevel TargetLevelData = Ach.Levels[TargetLevel];

                WriteInteger(Ach.Id); // ach id
                WriteInteger(TargetLevel); // target level
               WriteString(Ach.GroupName + TargetLevel); // badge
                WriteInteger(TargetLevelData.Requirement); // requirement
                WriteInteger(TargetLevelData.Requirement); // requirement
                WriteInteger(TargetLevelData.RewardPixels); // pixels
                WriteInteger(0); // ach score
                WriteInteger(UserData != null ? UserData.Progress : 0); // Current progress
                WriteBoolean(UserData != null ? (UserData.Level >= Ach.Levels.Count) : false); // Set 100% completed(??)
               WriteString(Ach.Category);
               WriteString("basejump");
                WriteInteger(0); // total levels
                WriteInteger(0);
            }
           WriteString("");
        }
    }
}
