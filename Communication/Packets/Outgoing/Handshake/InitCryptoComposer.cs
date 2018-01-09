namespace Plus.Communication.Packets.Outgoing.Handshake
{
    public class InitCryptoComposer : ServerPacket
    {
        public InitCryptoComposer(string Prime, string Generator)
            : base(ServerPacketHeader.InitCryptoMessageComposer)
        {
           WriteString(Prime);
           WriteString(Generator);
        }
    }
}