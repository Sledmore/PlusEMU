using Plus.HabboHotel.Groups;

namespace Plus.Communication.Packets.Outgoing.Groups
{
    class GroupFurniSettingsComposer : MessageComposer
    {
        public Group Group { get; }
        public int ItemId { get; }
        public int UserId { get; }

        public GroupFurniSettingsComposer(Group Group, int ItemId, int UserId)
            : base(ServerPacketHeader.GroupFurniSettingsMessageComposer)
        {
            this.Group = Group;
            this.ItemId = ItemId;
            this.UserId = UserId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(ItemId);//Item Id
            packet.WriteInteger(Group.Id);//Group Id?
            packet.WriteString(Group.Name);
            packet.WriteInteger(Group.RoomId);//RoomId
            packet.WriteBoolean(Group.IsMember(UserId));//Member?
            packet.WriteBoolean(Group.ForumEnabled);//Has a forum
        }
    }
}