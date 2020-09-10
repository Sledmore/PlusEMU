using System;

namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class MutedComposer : MessageComposer
    {
        public double TimeMuted { get; }

        public MutedComposer(double TimeMuted)
            : base(ServerPacketHeader.MutedMessageComposer)
        {
            this.TimeMuted = TimeMuted;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Convert.ToInt32(TimeMuted));
        }
    }
}
