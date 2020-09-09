using System.Collections.Generic;

using Plus.HabboHotel.Groups;

namespace Plus.Communication.Packets.Outgoing.Users
{
    class HabboGroupBadgesComposer : MessageComposer
    {
        public Dictionary<int, string> Badges { get; }
        public Group Group { get; }
        public HabboGroupBadgesComposer(Dictionary<int, string> badges)
            : base(ServerPacketHeader.HabboGroupBadgesMessageComposer)
        {
            this.Badges = badges;
        }

        public HabboGroupBadgesComposer(Group group)
            : base(ServerPacketHeader.HabboGroupBadgesMessageComposer)
        {
            this.Group = group;
        }

        public override void Compose(ServerPacket packet)
        {
            if(this.Badges != null)
            {
                packet.WriteInteger(Badges.Count);
                foreach (KeyValuePair<int, string> badge in Badges)
                {
                    packet.WriteInteger(badge.Key);
                    packet.WriteString(badge.Value);
                }
            } else if(Group != null)
            {
                packet.WriteInteger(1); //count

                packet.WriteInteger(Group.Id);
                packet.WriteString(Group.Badge);
            } else
            {
                packet.WriteInteger(0);
            }
        }
    }
}
