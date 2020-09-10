namespace Plus.Communication.Packets.Outgoing.Rooms.Session
{
    public class RoomForwardComposer : MessageComposer
    {
        public int RoomId { get; }
        public RoomForwardComposer(int RoomId)
            : base(ServerPacketHeader.RoomForwardMessageComposer)
        {
            this.RoomId = RoomId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(RoomId);
        }
    }
}