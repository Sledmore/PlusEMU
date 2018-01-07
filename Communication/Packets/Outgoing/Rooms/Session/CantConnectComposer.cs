namespace Plus.Communication.Packets.Outgoing.Rooms.Session
{
    class CantConnectComposer : ServerPacket
    {
        public CantConnectComposer(int Error)
            : base(ServerPacketHeader.CantConnectMessageComposer)
        {
            base.WriteInteger(Error);
        }
    }
}
