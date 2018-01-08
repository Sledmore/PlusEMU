namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class FloorHeightMapComposer : ServerPacket
    {
        public FloorHeightMapComposer(string Map, int WallHeight)
            : base(ServerPacketHeader.FloorHeightMapMessageComposer)
        {
            base.WriteBoolean(false);
            base.WriteInteger(WallHeight);
           base.WriteString(Map);
        }
    }
}
