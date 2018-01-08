namespace Plus.Communication.Packets.Outgoing.Rooms.Session
{
    public class RoomForwardComposer : ServerPacket
    {
        public RoomForwardComposer(int RoomId)
            : base(ServerPacketHeader.RoomForwardMessageComposer)
        {
            WriteInteger(RoomId);
        }
    }
}