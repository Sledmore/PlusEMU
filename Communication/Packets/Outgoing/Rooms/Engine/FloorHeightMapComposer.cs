namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class FloorHeightMapComposer : MessageComposer
    {
        public string Map { get; }
        public int WallHeight { get; }

        public FloorHeightMapComposer(string Map, int WallHeight)
            : base(ServerPacketHeader.FloorHeightMapMessageComposer)
        {
            this.Map = Map;
            this.WallHeight = WallHeight;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteBoolean(false);
            packet.WriteInteger(WallHeight);
            packet.WriteString(Map);
        }
    }
}
