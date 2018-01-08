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
            base.WriteInteger(GameId);
            base.WriteInteger(Achievements.Count);
            foreach (Achievement Ach in Achievements.ToList())
            {
                UserAchievement UserData = Session.GetHabbo().GetAchievementData(Ach.GroupName);
                int TargetLevel = (UserData != null ? UserData.Level + 1 : 1);
           
                AchievementLevel TargetLevelData = Ach.Levels[TargetLevel];

                base.WriteInteger(Ach.Id); // ach id
                base.WriteInteger(TargetLevel); // target level
               base.WriteString(Ach.GroupName + TargetLevel); // badge
                base.WriteInteger(TargetLevelData.Requirement); // requirement
                base.WriteInteger(TargetLevelData.Requirement); // requirement
                base.WriteInteger(TargetLevelData.RewardPixels); // pixels
                base.WriteInteger(0); // ach score
                base.WriteInteger(UserData != null ? UserData.Progress : 0); // Current progress
                base.WriteBoolean(UserData != null ? (UserData.Level >= Ach.Levels.Count) : false); // Set 100% completed(??)
               base.WriteString(Ach.Category);
               base.WriteString("basejump");
                base.WriteInteger(0); // total levels
                base.WriteInteger(0);
            }
           base.WriteString("");
        }
    }
}
