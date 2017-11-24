using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Users;
using Plus.HabboHotel.Users.Badges;

namespace Plus.Communication.Packets.Outgoing.Users
{
    class HabboUserBadgesComposer : ServerPacket
    {
        public HabboUserBadgesComposer(Habbo Habbo)
            : base(ServerPacketHeader.HabboUserBadgesMessageComposer)
        {
            base.WriteInteger(Habbo.Id);
            base.WriteInteger(Habbo.GetBadgeComponent().EquippedCount);

            foreach (Badge Badge in Habbo.GetBadgeComponent().GetBadges().ToList())
            {
                if (Badge.Slot <= 0)
                    continue;

                base.WriteInteger(Badge.Slot);
               base.WriteString(Badge.Code);
            }
        }
    }
}
