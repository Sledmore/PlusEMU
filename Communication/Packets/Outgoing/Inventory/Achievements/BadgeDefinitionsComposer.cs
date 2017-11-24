using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Plus.HabboHotel.Achievements;

namespace Plus.Communication.Packets.Outgoing.Inventory.Achievements
{
    class BadgeDefinitionsComposer: ServerPacket
    {
        public BadgeDefinitionsComposer(Dictionary<string, Achievement> Achievements)
            : base(ServerPacketHeader.BadgeDefinitionsMessageComposer)
        {
            base.WriteInteger(Achievements.Count);

            foreach (Achievement Achievement in Achievements.Values)
            {
               base.WriteString(Achievement.GroupName.Replace("ACH_", ""));
                base.WriteInteger(Achievement.Levels.Count);
                foreach (AchievementLevel Level in Achievement.Levels.Values)
                {
                    base.WriteInteger(Level.Level);
                    base.WriteInteger(Level.Requirement);
                }
            }
        }
    }
}
