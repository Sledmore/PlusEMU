using Plus.HabboHotel.Items;

namespace Plus.HabboHotel.Catalog.Utilities
{
    public static class ItemUtility
    {
        public static bool CanGiftItem(CatalogItem Item)
        {
            if (!Item.Data.AllowGift || Item.IsLimited || Item.Amount > 1 || Item.Data.InteractionType == InteractionType.EXCHANGE ||
                Item.Data.InteractionType == InteractionType.BADGE || (Item.Data.Type != 's' && Item.Data.Type != 'i') || Item.CostDiamonds > 0 ||
                Item.Data.InteractionType == InteractionType.TELEPORT || Item.Data.InteractionType == InteractionType.DEAL)
                return false;

            if (Item.Data.IsRare)
                return false;

            if (Item.Data.InteractionType == InteractionType.PET)
                return false;
            return true;
        }

        public static bool CanSelectAmount(CatalogItem Item)
        {
            if (Item.IsLimited || Item.Amount > 1 || Item.Data.InteractionType == InteractionType.EXCHANGE || !Item.HaveOffer || Item.Data.InteractionType == InteractionType.BADGE || Item.Data.InteractionType == InteractionType.DEAL)
                return false;
            return true;
        }

        public static int GetSaddleId(int Saddle)
        {
            switch (Saddle)
            {
                default:
                case 9:
                    return 4221;
                case 10:
                    return 4450;
            }
        }

        public static bool IsRare(Item Item)
        {
            if (Item.LimitedNo > 0)
                return true;

            if (Item.Data.IsRare)
                return true;

            return false;
        }
    }
}