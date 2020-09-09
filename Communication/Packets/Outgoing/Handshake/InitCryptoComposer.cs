namespace Plus.Communication.Packets.Outgoing.Handshake
{
    public class InitCryptoComposer : MessageComposer
    {
        public string Prime { get; }
        public string Generator { get; }

        public InitCryptoComposer(string Prime, string Generator)
            : base(ServerPacketHeader.InitCryptoMessageComposer)
        {
            this.Prime = Prime;
            this.Generator = Generator;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(Prime);
            packet.WriteString(Generator);
        }
    }
}