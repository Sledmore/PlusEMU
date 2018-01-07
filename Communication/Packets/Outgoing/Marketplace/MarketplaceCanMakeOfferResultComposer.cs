namespace Plus.Communication.Packets.Outgoing.Marketplace
{
    class MarketplaceCanMakeOfferResultComposer : ServerPacket
    {
        public MarketplaceCanMakeOfferResultComposer(int Result)
            : base(ServerPacketHeader.MarketplaceCanMakeOfferResultMessageComposer)
        {
            base.WriteInteger(Result);
            base.WriteInteger(0);
        }
    }
}
