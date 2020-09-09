namespace Plus.Communication.Packets.Outgoing.Handshake
{
    class PongComposer : MessageComposer
    {
        public PongComposer()
            : base(ServerPacketHeader.PongMessageComposer)
        {

        }

        public override void Compose(ServerPacket packet)
        {
            
        }
    }
}
