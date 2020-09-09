namespace Plus.Communication.Packets.Outgoing.Rooms.Session
{
    class CantConnectComposer : MessageComposer
    {
        public int Error { get; }
        public CantConnectComposer(int Error)
            : base(ServerPacketHeader.CantConnectMessageComposer)
        {
            this.Error = Error;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Error);
        }
    }
}
