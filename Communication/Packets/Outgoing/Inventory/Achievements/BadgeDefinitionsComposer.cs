using System.Collections.Generic;
using Plus.HabboHotel.Achievements;

namespace Plus.Communication.Packets.Outgoing.Inventory.Achievements
{
    class BadgeDefinitionsComposer : MessageComposer
    {
        public Dictionary<string, Achievement> Achievements { get; }

        public BadgeDefinitionsComposer(Dictionary<string, Achievement> Achievements)
            : base(ServerPacketHeader.BadgeDefinitionsMessageComposer)
        {
            this.Achievements = Achievements;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Achievements.Count);

            foreach (Achievement Achievement in Achievements.Values)
            {
                packet.WriteString(Achievement.GroupName.Replace("ACH_", ""));
                packet.WriteInteger(Achievement.Levels.Count);
                foreach (AchievementLevel Level in Achievement.Levels.Values)
                {
                    packet.WriteInteger(Level.Level);
                    packet.WriteInteger(Level.Requirement);
                }
            }
        }
    }
}
