using System.Collections.Generic;

using Plus.HabboHotel.Groups;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class GroupFurniConfigComposer : ServerPacket
    {
        public GroupFurniConfigComposer(ICollection<Group> groups)
            : base(ServerPacketHeader.GroupFurniConfigMessageComposer)
        {
            base.WriteInteger(groups.Count);
            foreach (Group group in groups)
            {
                base.WriteInteger(group.Id);
                base.WriteString(group.Name);
                base.WriteString(group.Badge);
                base.WriteString(PlusEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour1, true));
                base.WriteString(PlusEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour2, false));
                base.WriteBoolean(false);
                base.WriteInteger(group.CreatorId);
                base.WriteBoolean(group.ForumEnabled);
            }
        }
    }
}
