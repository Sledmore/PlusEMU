namespace Plus.Communication.Packets.Outgoing.Rooms.Session
{
    class RoomReadyComposer : ServerPacket
    {
        public RoomReadyComposer(int RoomId, string Model)
            : base(ServerPacketHeader.RoomReadyMessageComposer)
        {
           WriteString(Model);
            WriteInteger(RoomId);
        }
    }
}
