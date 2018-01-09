namespace Plus.Communication.Packets.Outgoing.Marketplace
{
    class MarketplaceItemStatsComposer : ServerPacket
    {
        public MarketplaceItemStatsComposer(int ItemId, int SpriteId, int AveragePrice)
            : base(ServerPacketHeader.MarketplaceItemStatsMessageComposer)
        {
            WriteInteger(AveragePrice);//Avg price in last 7 days.
            WriteInteger(PlusEnvironment.GetGame().GetCatalog().GetMarketplace().OfferCountForSprite(SpriteId));

            WriteInteger(0);//No idea.
            WriteInteger(0);//No idea.

            WriteInteger(ItemId);
            WriteInteger(SpriteId);
        }
    }
}