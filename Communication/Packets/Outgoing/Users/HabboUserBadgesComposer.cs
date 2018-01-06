using System.Linq;

using Plus.HabboHotel.Users;
using Plus.HabboHotel.Users.Badges;

namespace Plus.Communication.Packets.Outgoing.Users
{
    class HabboUserBadgesComposer : ServerPacket
    {
        public HabboUserBadgesComposer(Habbo habbo)
            : base(ServerPacketHeader.HabboUserBadgesMessageComposer)
        {
            WriteInteger(habbo.Id);
            WriteInteger(habbo.GetBadgeComponent().EquippedCount);

            foreach (Badge badge in habbo.GetBadgeComponent().GetBadges().Where(b => b.Slot > 0).ToList())
            {
                WriteInteger(badge.Slot);
                WriteString(badge.Code);
            }
        }
    }
}
