namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class RoomPropertyComposer : ServerPacket
    {
        public RoomPropertyComposer(string name, string val)
            : base(ServerPacketHeader.RoomPropertyMessageComposer)
        {
           WriteString(name);
           WriteString(val);
        }
    }
}
