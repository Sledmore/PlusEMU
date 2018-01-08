using System.Collections.Generic;
using Plus.HabboHotel.Achievements;

namespace Plus.Communication.Packets.Outgoing.Inventory.Achievements
{
    class BadgeDefinitionsComposer: ServerPacket
    {
        public BadgeDefinitionsComposer(Dictionary<string, Achievement> Achievements)
            : base(ServerPacketHeader.BadgeDefinitionsMessageComposer)
        {
            WriteInteger(Achievements.Count);

            foreach (Achievement Achievement in Achievements.Values)
            {
               WriteString(Achievement.GroupName.Replace("ACH_", ""));
                WriteInteger(Achievement.Levels.Count);
                foreach (AchievementLevel Level in Achievement.Levels.Values)
                {
                    WriteInteger(Level.Level);
                    WriteInteger(Level.Requirement);
                }
            }
        }
    }
}
