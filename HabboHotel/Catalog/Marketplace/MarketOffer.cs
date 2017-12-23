namespace Plus.HabboHotel.Catalog.Marketplace
{
    public class MarketOffer
    {
        public int OfferID { get; private set; }
        public int ItemType { get; private set; }
        public int SpriteId { get; private set; }
        public int TotalPrice { get; private set; }
        public int LimitedNumber { get; private set; }
        public int LimitedStack { get; private set; }

        public MarketOffer(int offerId, int spriteId, int totalPrice, int itemType, int limitedNumber, int limitedStack)
        {
            OfferID = offerId;
            SpriteId = spriteId;
            ItemType = itemType;
            TotalPrice = totalPrice;
            LimitedNumber = limitedNumber;
            LimitedStack = limitedStack;
        }
    }
}
