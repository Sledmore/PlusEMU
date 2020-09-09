namespace Plus.Communication.Packets.Outgoing.Marketplace
{
    class MarketplaceCancelOfferResultComposer : MessageComposer
    {
        public int OfferId { get; }
        public bool Success { get; }

        public MarketplaceCancelOfferResultComposer(int OfferId, bool Success)
            : base(ServerPacketHeader.MarketplaceCancelOfferResultMessageComposer)
        {
            this.OfferId = OfferId;
            this.Success = Success;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(OfferId);
            packet.WriteBoolean(Success);
        }
    }
}
