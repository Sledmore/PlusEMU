namespace Plus.HabboHotel.Catalog.Marketplace
{
    public class MarketOffer
    {
        public int OfferID { get; set; }
        public int ItemType { get; set; }
        public int SpriteId { get; set; }
        public int TotalPrice { get; set; }
        public int LimitedNumber { get; set; }
        public int LimitedStack { get; set; }

        public MarketOffer(int OfferID, int SpriteId, int TotalPrice, int ItemType, int LimitedNumber, int LimitedStack)
        {
            this.OfferID = OfferID;
            this.SpriteId = SpriteId;
            this.ItemType = ItemType;
            this.TotalPrice = TotalPrice;
            this.LimitedNumber = LimitedNumber;
            this.LimitedStack = LimitedStack;
        }
    }
}
