using Plus.HabboHotel.Items;

namespace Plus.HabboHotel.Catalog
{
    public class CatalogItem
    {
        public int Id { get; private set; }
        public int ItemId { get; private set; }
        public ItemData Data { get; private set; }
        public int Amount { get; private set; }
        public int CostCredits { get; private set; }
        public string ExtraData { get; private set; }
        public bool HaveOffer { get; private set; }
        public bool IsLimited { get; private set; }
        public string Name { get; private set; }
        public int PageID { get; private set; }
        public int CostPixels { get; private set; }
        public int LimitedEditionStack { get; private set; }
        public int LimitedEditionSells { get; set; }
        public int CostDiamonds { get; private set; }
        public string Badge { get; private set; }
        public int OfferId { get; private set; }

        public CatalogItem(int id, int itemId, ItemData data, string catalogName, int pageId, int costCredits, int costPixels,
            int costDiamonds, int amount, int limitedEditionSells, int limitedEditionStack, bool hasOffer, string extraData, string badge, int offerId)
        {
            Id = id;
            Name = catalogName;
            ItemId = itemId;
            Data = data;
            PageID = pageId;
            CostCredits = costCredits;
            CostPixels = costPixels;
            CostDiamonds = costDiamonds;
            Amount = amount;
            LimitedEditionSells = limitedEditionSells;
            LimitedEditionStack = limitedEditionStack;
            IsLimited = (limitedEditionStack > 0);
            HaveOffer = hasOffer;
            ExtraData = extraData;
            Badge = badge;
            OfferId = offerId;
        }
    }
}