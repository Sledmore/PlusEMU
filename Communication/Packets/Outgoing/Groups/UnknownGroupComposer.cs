namespace Plus.Communication.Packets.Outgoing.Groups
{
    class UnknownGroupComposer : MessageComposer
    {
        public int GroupId { get; }
        public int HabboId { get; }

        public UnknownGroupComposer(int GroupId, int HabboId)
            : base(ServerPacketHeader.UnknownGroupMessageComposer)
        {
            this.GroupId = GroupId;
            this.HabboId = HabboId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(GroupId);
            packet.WriteInteger(HabboId);
        }
    }
}