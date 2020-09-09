using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users.Badges;

namespace Plus.Communication.Packets.Outgoing.Inventory.Badges
{
    class BadgesComposer : MessageComposer
    {
        public ICollection<Badge> Badges { get; }

        public BadgesComposer(ICollection<Badge> Badges)
            : base(ServerPacketHeader.BadgesMessageComposer)
        {
            this.Badges = Badges;
        }

        public override void Compose(ServerPacket packet)
        {
            List<Badge> EquippedBadges = new List<Badge>();

            packet.WriteInteger(Badges.Count);
            foreach (Badge Badge in Badges)
            {
                packet.WriteInteger(1);
                packet.WriteString(Badge.Code);

                if (Badge.Slot > 0)
                    EquippedBadges.Add(Badge);
            }

            packet.WriteInteger(EquippedBadges.Count);
            foreach (Badge Badge in EquippedBadges)
            {
                packet.WriteInteger(Badge.Slot);
                packet.WriteString(Badge.Code);
            }
        }
    }
}
