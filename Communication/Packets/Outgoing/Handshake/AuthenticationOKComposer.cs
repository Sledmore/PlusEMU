namespace Plus.Communication.Packets.Outgoing.Handshake
{
    public class AuthenticationOKComposer : MessageComposer
    {
        public AuthenticationOKComposer()
            : base(ServerPacketHeader.AuthenticationOKMessageComposer)
        {
        }

        public override void Compose(ServerPacket packet)
        {
            
        }
    }
}