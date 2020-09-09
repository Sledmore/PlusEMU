namespace Plus.Communication.Packets.Outgoing.Rooms.Session
{
    class CloseConnectionComposer : MessageComposer
    {
        public CloseConnectionComposer()
            : base(ServerPacketHeader.CloseConnectionMessageComposer)
        {

        }

        public override void Compose(ServerPacket packet)
        {
            
        }
    }
}
