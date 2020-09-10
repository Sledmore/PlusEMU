using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.Groups
{
    class GroupMembershipRequestedComposer : MessageComposer
    {
        public int GroupId { get; }
        public Habbo Habbo { get; }
        public int Type { get; }

        public GroupMembershipRequestedComposer(int GroupId, Habbo Habbo, int Type) 
            : base(ServerPacketHeader.GroupMembershipRequestedMessageComposer)
        {
            this.GroupId = GroupId;
            this.Habbo = Habbo;
            this.Type = Type;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(GroupId);//GroupId
            packet.WriteInteger(Type);//Type?
            {
                packet.WriteInteger(Habbo.Id);//UserId
                packet.WriteString(Habbo.Username);
                packet.WriteString(Habbo.Look);
                packet.WriteString(string.Empty);
            }
        }
    }
}
