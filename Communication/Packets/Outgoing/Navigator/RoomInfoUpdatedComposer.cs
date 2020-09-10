namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class RoomInfoUpdatedComposer : MessageComposer
    {
        public int RoomId { get; }

        public RoomInfoUpdatedComposer(int roomId)
            : base(ServerPacketHeader.RoomInfoUpdatedMessageComposer)
        {
            this.RoomId = roomId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(RoomId);
        }
    }
}
