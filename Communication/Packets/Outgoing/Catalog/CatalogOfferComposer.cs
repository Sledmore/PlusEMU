using Plus.HabboHotel.Items;
using Plus.HabboHotel.Catalog;
using Plus.HabboHotel.Catalog.Utilities;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class CatalogOfferComposer : ServerPacket
    {
        public CatalogOfferComposer(CatalogItem Item)
            : base(ServerPacketHeader.CatalogOfferMessageComposer)
        {
            base.WriteInteger(Item.OfferId);
            base.WriteString(Item.Data.ItemName);
            base.WriteBoolean(false);//IsRentable
            base.WriteInteger(Item.CostCredits);

            if (Item.CostDiamonds > 0)
            {
                base.WriteInteger(Item.CostDiamonds);
                base.WriteInteger(5); // Diamonds
            }
            else
            {
                base.WriteInteger(Item.CostPixels);
                base.WriteInteger(0); // Type of PixelCost
            }

            base.WriteBoolean(ItemUtility.CanGiftItem(Item));
            base.WriteInteger(string.IsNullOrEmpty(Item.Badge) ? 1 : 2);//Count 1 item if there is no badge, otherwise count as 2.

            if (!string.IsNullOrEmpty(Item.Badge))
            {
                base.WriteString("b");
                base.WriteString(Item.Badge);
            }

            base.WriteString(Item.Data.Type.ToString());
            if (Item.Data.Type.ToString().ToLower() == "b")
                base.WriteString(Item.Data.ItemName);//Badge name.
            else
            {
                base.WriteInteger(Item.Data.SpriteId);
                if (Item.Data.InteractionType == InteractionType.WALLPAPER || Item.Data.InteractionType == InteractionType.FLOOR || Item.Data.InteractionType == InteractionType.LANDSCAPE)
                    base.WriteString(Item.Name.Split('_')[2]);
                else if (Item.PageID == 9)//Bots
                {
                    CatalogBot CataBot = null;
                    if (!PlusEnvironment.GetGame().GetCatalog().TryGetBot(Item.ItemId, out CataBot))
                        base.WriteString("hd-180-7.ea-1406-62.ch-210-1321.hr-831-49.ca-1813-62.sh-295-1321.lg-285-92");
                    else
                        base.WriteString(CataBot.Figure);
                }
                else if (Item.ExtraData != null)
                    base.WriteString(Item.ExtraData != null ? Item.ExtraData : string.Empty);
                base.WriteInteger(Item.Amount);
                base.WriteBoolean(Item.IsLimited); // IsLimited
                if (Item.IsLimited)
                {
                    base.WriteInteger(Item.LimitedEditionStack);
                    base.WriteInteger(Item.LimitedEditionStack - Item.LimitedEditionSells);
                }
            }
            base.WriteInteger(0); // club_level
            base.WriteBoolean(ItemUtility.CanSelectAmount(Item));
            base.WriteBoolean(false);// TODO: Figure out
            base.WriteString("");//previewImage -> e.g; catalogue/pet_lion.png
        }
    }
}
