using System;

namespace Plus.Communication.Packets.Outgoing.Handshake
{
    public class InitCryptoComposer : ServerPacket
    {
        public InitCryptoComposer(string Prime, string Generator)
            : base(ServerPacketHeader.InitCryptoMessageComposer)
        {
           base.WriteString(Prime);
           base.WriteString(Generator);
        }
    }
}