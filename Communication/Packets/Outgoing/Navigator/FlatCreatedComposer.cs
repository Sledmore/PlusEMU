namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class FlatCreatedComposer : MessageComposer
    {
        public int RoomId { get; }
        public string RoomName { get; }

        public FlatCreatedComposer(int roomId, string roomName)
            : base(ServerPacketHeader.FlatCreatedMessageComposer)
        {
            this.RoomId = roomId;
            this.RoomName = roomName;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(RoomId);
            packet.WriteString(RoomName);
        }
    }
}
