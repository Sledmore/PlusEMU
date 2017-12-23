using Plus.HabboHotel.Items;

namespace Plus.HabboHotel.Catalog.Utilities
{
    public static class ItemUtility
    {
        public static bool CanGiftItem(CatalogItem item)
        {
            if (!item.Data.AllowGift || item.IsLimited || item.Amount > 1 || item.Data.InteractionType == InteractionType.EXCHANGE ||
                item.Data.InteractionType == InteractionType.BADGE || (item.Data.Type != 's' && item.Data.Type != 'i') || item.CostDiamonds > 0 ||
                item.Data.InteractionType == InteractionType.TELEPORT || item.Data.InteractionType == InteractionType.DEAL)
                return false;

            if (item.Data.IsRare)
                return false;

            if (item.Data.InteractionType == InteractionType.PET)
                return false;
            return true;
        }

        public static bool CanSelectAmount(CatalogItem item)
        {
            if (item.IsLimited || item.Amount > 1 || item.Data.InteractionType == InteractionType.EXCHANGE || !item.HaveOffer || item.Data.InteractionType == InteractionType.BADGE || item.Data.InteractionType == InteractionType.DEAL)
                return false;
            return true;
        }

        public static int GetSaddleId(int saddle)
        {
            switch (saddle)
            {
                default:
                case 9:
                    return 4221;
                case 10:
                    return 4450;
            }
        }

        public static bool IsRare(Item item)
        {
            if (item.LimitedNo > 0)
                return true;

            if (item.Data.IsRare)
                return true;

            return false;
        }
    }
}