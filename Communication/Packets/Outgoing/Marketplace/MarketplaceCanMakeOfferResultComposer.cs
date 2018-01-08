namespace Plus.Communication.Packets.Outgoing.Marketplace
{
    class MarketplaceCanMakeOfferResultComposer : ServerPacket
    {
        public MarketplaceCanMakeOfferResultComposer(int Result)
            : base(ServerPacketHeader.MarketplaceCanMakeOfferResultMessageComposer)
        {
            WriteInteger(Result);
            WriteInteger(0);
        }
    }
}
