namespace Plus.Communication.Packets.Outgoing.Handshake
{
    class AvailabilityStatusComposer : MessageComposer
    {
        public AvailabilityStatusComposer()
            : base(ServerPacketHeader.AvailabilityStatusMessageComposer)
        {
            
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteBoolean(true);
            packet.WriteBoolean(false);
            packet.WriteBoolean(true);
        }
    }
}
