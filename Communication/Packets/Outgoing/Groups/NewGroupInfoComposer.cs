namespace Plus.Communication.Packets.Outgoing.Groups
{
    class NewGroupInfoComposer : MessageComposer
    {
        public int RoomId { get; }
        public int GroupId { get; }

        public NewGroupInfoComposer(int RoomId, int GroupId)
            : base(ServerPacketHeader.NewGroupInfoMessageComposer)
        {
            this.RoomId = RoomId;
            this.GroupId = GroupId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(RoomId);
            packet.WriteInteger(GroupId);
        }
    }
}
