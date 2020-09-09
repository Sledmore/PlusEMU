namespace Plus.Communication.Packets.Outgoing.Rooms.Session
{
    class RoomReadyComposer : MessageComposer
    {
        public int RoomId { get; }
        public string Model { get; }
        public RoomReadyComposer(int RoomId, string Model)
            : base(ServerPacketHeader.RoomReadyMessageComposer)
        {
            this.RoomId = RoomId;
            this.Model = Model;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(Model);
            packet.WriteInteger(RoomId);
        }
    }
}
