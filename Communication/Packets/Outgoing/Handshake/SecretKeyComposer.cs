namespace Plus.Communication.Packets.Outgoing.Handshake
{
    public class SecretKeyComposer : MessageComposer
    {
        public string PublicKey { get; }

        public SecretKeyComposer(string publicKey)
            : base(ServerPacketHeader.SecretKeyMessageComposer)
        {
            this.PublicKey = publicKey;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(PublicKey);
        }
    }
}