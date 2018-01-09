using System;

namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class MutedComposer : ServerPacket
    {
        public MutedComposer(double TimeMuted)
            : base(ServerPacketHeader.MutedMessageComposer)
        {
            WriteInteger(Convert.ToInt32(TimeMuted));
        }
    }
}
