using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Achievements;
using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.GameCenter
{
    class GameAchievementListComposer : MessageComposer
    {
        public Habbo Habbo { get; }
        public ICollection<Achievement> Achievements { get; }
        public int GameId { get; }

        public GameAchievementListComposer(GameClient Session, ICollection<Achievement> Achievements, int GameId)
            : base(ServerPacketHeader.GameAchievementListMessageComposer)
        {
            this.Habbo = Session.GetHabbo();
            this.Achievements = Achievements;
            this.GameId = GameId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(GameId);
            packet.WriteInteger(Achievements.Count);
            foreach (Achievement Ach in Achievements.ToList())
            {
                UserAchievement UserData = Habbo.GetAchievementData(Ach.GroupName);
                int TargetLevel = (UserData != null ? UserData.Level + 1 : 1);

                AchievementLevel TargetLevelData = Ach.Levels[TargetLevel];

                packet.WriteInteger(Ach.Id); // ach id
                packet.WriteInteger(TargetLevel); // target level
                packet.WriteString(Ach.GroupName + TargetLevel); // badge
                packet.WriteInteger(TargetLevelData.Requirement); // requirement
                packet.WriteInteger(TargetLevelData.Requirement); // requirement
                packet.WriteInteger(TargetLevelData.RewardPixels); // pixels
                packet.WriteInteger(0); // ach score
                packet.WriteInteger(UserData != null ? UserData.Progress : 0); // Current progress
                packet.WriteBoolean(UserData != null ? (UserData.Level >= Ach.Levels.Count) : false); // Set 100% completed(??)
                packet.WriteString(Ach.Category);
                packet.WriteString("basejump");
                packet.WriteInteger(0); // total levels
                packet.WriteInteger(0);
            }
            packet.WriteString("");
        }
    }
}
