using Plus.HabboHotel.Items;
using Plus.HabboHotel.Catalog;
using Plus.HabboHotel.Catalog.Utilities;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class CatalogOfferComposer : MessageComposer
    {
        public CatalogItem Item { get; }

        public CatalogOfferComposer(CatalogItem Item)
            : base(ServerPacketHeader.CatalogOfferMessageComposer)
        {
            this.Item = Item;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Item.OfferId);
            packet.WriteString(Item.Data.ItemName);
            packet.WriteBoolean(false);//IsRentable
            packet.WriteInteger(Item.CostCredits);

            if (Item.CostDiamonds > 0)
            {
                packet.WriteInteger(Item.CostDiamonds);
                packet.WriteInteger(5); // Diamonds
            }
            else
            {
                packet.WriteInteger(Item.CostPixels);
                packet.WriteInteger(0); // Type of PixelCost
            }

            packet.WriteBoolean(ItemUtility.CanGiftItem(Item));
            packet.WriteInteger(string.IsNullOrEmpty(Item.Badge) ? 1 : 2);//Count 1 item if there is no badge, otherwise count as 2.

            if (!string.IsNullOrEmpty(Item.Badge))
            {
                packet.WriteString("b");
                packet.WriteString(Item.Badge);
            }

            packet.WriteString(Item.Data.Type.ToString());
            if (Item.Data.Type.ToString().ToLower() == "b")
                packet.WriteString(Item.Data.ItemName);//Badge name.
            else
            {
                packet.WriteInteger(Item.Data.SpriteId);
                if (Item.Data.InteractionType == InteractionType.WALLPAPER || Item.Data.InteractionType == InteractionType.FLOOR || Item.Data.InteractionType == InteractionType.LANDSCAPE)
                    packet.WriteString(Item.Name.Split('_')[2]);
                else if (Item.PageID == 9)//Bots
                {
                    CatalogBot CataBot = null;
                    if (!PlusEnvironment.GetGame().GetCatalog().TryGetBot(Item.ItemId, out CataBot))
                        packet.WriteString("hd-180-7.ea-1406-62.ch-210-1321.hr-831-49.ca-1813-62.sh-295-1321.lg-285-92");
                    else
                        packet.WriteString(CataBot.Figure);
                }
                else if (Item.ExtraData != null)
                    packet.WriteString(Item.ExtraData != null ? Item.ExtraData : string.Empty);
                packet.WriteInteger(Item.Amount);
                packet.WriteBoolean(Item.IsLimited); // IsLimited
                if (Item.IsLimited)
                {
                    packet.WriteInteger(Item.LimitedEditionStack);
                    packet.WriteInteger(Item.LimitedEditionStack - Item.LimitedEditionSells);
                }
            }
            packet.WriteInteger(0); // club_level
            packet.WriteBoolean(ItemUtility.CanSelectAmount(Item));
            packet.WriteBoolean(false);// TODO: Figure out
            packet.WriteString("");//previewImage -> e.g; catalogue/pet_lion.png
        }
    }
}
