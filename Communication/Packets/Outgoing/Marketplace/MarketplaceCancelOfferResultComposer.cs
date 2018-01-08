namespace Plus.Communication.Packets.Outgoing.Marketplace
{
    class MarketplaceCancelOfferResultComposer : ServerPacket
    {
        public MarketplaceCancelOfferResultComposer(int OfferId, bool Success)
            : base(ServerPacketHeader.MarketplaceCancelOfferResultMessageComposer)
        {
            WriteInteger(OfferId);
            WriteBoolean(Success);
        }
    }
}
