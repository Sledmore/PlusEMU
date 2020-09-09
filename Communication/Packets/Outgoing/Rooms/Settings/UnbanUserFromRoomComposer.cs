namespace Plus.Communication.Packets.Outgoing.Rooms.Settings
{
    class UnbanUserFromRoomComposer : MessageComposer
    {
        public int RoomId { get; }
        public int UserId { get; }
        public UnbanUserFromRoomComposer(int RoomId, int UserId)
            : base(ServerPacketHeader.UnbanUserFromRoomMessageComposer)
        {
            this.RoomId = RoomId;
            this.UserId = UserId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(RoomId);
            packet.WriteInteger(UserId);
        }
    }
}
