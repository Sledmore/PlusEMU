namespace Plus.Communication.Packets.Outgoing.Marketplace
{
    class MarketplaceItemStatsComposer : MessageComposer
    {
        public int ItemId { get; }
        public int SpriteId { get; }
        public int AveragePrice { get; }

        public MarketplaceItemStatsComposer(int ItemId, int SpriteId, int AveragePrice)
            : base(ServerPacketHeader.MarketplaceItemStatsMessageComposer)
        {
            this.ItemId = ItemId;
            this.SpriteId = SpriteId;
            this.AveragePrice = AveragePrice;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(AveragePrice);//Avg price in last 7 days.
            packet.WriteInteger(PlusEnvironment.GetGame().GetCatalog().GetMarketplace().OfferCountForSprite(SpriteId));

            packet.WriteInteger(0);//No idea.
            packet.WriteInteger(0);//No idea.

            packet.WriteInteger(ItemId);
            packet.WriteInteger(SpriteId);
        }
    }
}