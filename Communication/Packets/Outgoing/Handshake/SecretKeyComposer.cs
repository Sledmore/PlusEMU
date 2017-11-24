namespace Plus.Communication.Packets.Outgoing.Handshake
{
    public class SecretKeyComposer : ServerPacket
    {
        public SecretKeyComposer(string PublicKey)
            : base(ServerPacketHeader.SecretKeyMessageComposer)
        {
           base.WriteString(PublicKey);
        }
    }
}