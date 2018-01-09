namespace Plus.Communication.Packets.Outgoing.Handshake
{
    public class SecretKeyComposer : ServerPacket
    {
        public SecretKeyComposer(string publicKey)
            : base(ServerPacketHeader.SecretKeyMessageComposer)
        {
           WriteString(publicKey);
        }
    }
}