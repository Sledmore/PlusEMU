using System;

namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class MutedComposer : ServerPacket
    {
        public MutedComposer(double TimeMuted)
            : base(ServerPacketHeader.MutedMessageComposer)
        {
            base.WriteInteger(Convert.ToInt32(TimeMuted));
        }
    }
}
