using System.Collections.Generic;

using Plus.HabboHotel.Groups;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class GroupFurniConfigComposer : MessageComposer
    {
        public ICollection<Group> Groups { get; }

        public GroupFurniConfigComposer(ICollection<Group> groups)
            : base(ServerPacketHeader.GroupFurniConfigMessageComposer)
        {
            this.Groups = groups;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Groups.Count);
            foreach (Group group in Groups)
            {
                packet.WriteInteger(group.Id);
                packet.WriteString(group.Name);
                packet.WriteString(group.Badge);
                packet.WriteString(PlusEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour1, true));
                packet.WriteString(PlusEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour2, false));
                packet.WriteBoolean(false);
                packet.WriteInteger(group.CreatorId);
                packet.WriteBoolean(group.ForumEnabled);
            }
        }
    }
}
