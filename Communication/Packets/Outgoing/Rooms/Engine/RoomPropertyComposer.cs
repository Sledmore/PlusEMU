namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class RoomPropertyComposer : MessageComposer
    {
        public string Name { get; }
        public string Val { get; }

        public RoomPropertyComposer(string name, string val)
            : base(ServerPacketHeader.RoomPropertyMessageComposer)
        {
            this.Name = name;
            this.Val = val;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(Name);
            packet.WriteString(Val);
        }
    }
}
