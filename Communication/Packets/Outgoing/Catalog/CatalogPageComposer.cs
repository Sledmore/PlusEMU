using System.Linq;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Catalog;
using Plus.HabboHotel.Catalog.Utilities;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class CatalogPageComposer : MessageComposer
    {
        public CatalogPage Page { get; }
        public string CatalogueMode { get; }

        public CatalogPageComposer(CatalogPage Page, string CataMode)
            : base(ServerPacketHeader.CatalogPageMessageComposer)
        {
            this.Page = Page;
            this.CatalogueMode = CataMode;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Page.Id);
            packet.WriteString(CatalogueMode);
            packet.WriteString(Page.Template);

            packet.WriteInteger(Page.PageStrings1.Count);
            foreach (string s in Page.PageStrings1)
            {
                packet.WriteString(s);
            }

            packet.WriteInteger(Page.PageStrings2.Count);
            foreach (string s in Page.PageStrings2)
            {
                packet.WriteString(s);
            }

            if (!Page.Template.Equals("frontpage") && !Page.Template.Equals("club_buy"))
            {
                packet.WriteInteger(Page.Items.Count);
                foreach (CatalogItem Item in Page.Items.Values)
                {
                    packet.WriteInteger(Item.Id);
                    packet.WriteString(Item.Name);
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

                    if (Item.Data.InteractionType == InteractionType.DEAL || Item.Data.InteractionType == InteractionType.ROOMDEAL)
                    {
                        CatalogDeal deal = null;
                        if (!PlusEnvironment.GetGame().GetCatalog().TryGetDeal(Item.Data.BehaviourData, out deal))
                        {
                            packet.WriteInteger(0);//Count
                        }
                        else
                        {
                            packet.WriteInteger(deal.ItemDataList.Count);

                            foreach (CatalogItem dealItem in deal.ItemDataList.ToList())
                            {
                                packet.WriteString(dealItem.Data.Type.ToString());
                                packet.WriteInteger(dealItem.Data.SpriteId);
                                packet.WriteString("");
                                packet.WriteInteger(dealItem.Amount);
                                packet.WriteBoolean(false);
                            }
                        }
                    }
                    else
                    {
                        packet.WriteInteger(string.IsNullOrEmpty(Item.Badge) ? 1 : 2);//Count 1 item if there is no badge, otherwise count as 2.

                        if (!string.IsNullOrEmpty(Item.Badge))
                        {
                            packet.WriteString("b");
                            packet.WriteString(Item.Badge);
                        }

                        packet.WriteString(Item.Data.Type.ToString());
                        if (Item.Data.Type.ToString().ToLower() == "b")
                        {
                            //This is just a badge, append the name.
                            packet.WriteString(Item.Data.ItemName);
                        }
                        else
                        {
                            packet.WriteInteger(Item.Data.SpriteId);
                            if (Item.Data.InteractionType == InteractionType.WALLPAPER || Item.Data.InteractionType == InteractionType.FLOOR || Item.Data.InteractionType == InteractionType.LANDSCAPE)
                            {
                                packet.WriteString(Item.Name.Split('_')[2]);
                            }
                            else if (Item.Data.InteractionType == InteractionType.BOT)//Bots
                            {
                                CatalogBot CatalogBot = null;
                                if (!PlusEnvironment.GetGame().GetCatalog().TryGetBot(Item.ItemId, out CatalogBot))
                                    packet.WriteString("hd-180-7.ea-1406-62.ch-210-1321.hr-831-49.ca-1813-62.sh-295-1321.lg-285-92");
                                else
                                    packet.WriteString(CatalogBot.Figure);
                            }
                            else if (Item.ExtraData != null)
                            {
                                packet.WriteString(Item.ExtraData != null ? Item.ExtraData : string.Empty);
                            }
                            packet.WriteInteger(Item.Amount);
                            packet.WriteBoolean(Item.IsLimited); // IsLimited
                            if (Item.IsLimited)
                            {
                                packet.WriteInteger(Item.LimitedEditionStack);
                                packet.WriteInteger(Item.LimitedEditionStack - Item.LimitedEditionSells);
                            }
                        }
                    }
                    packet.WriteInteger(0); //club_level
                    packet.WriteBoolean(ItemUtility.CanSelectAmount(Item));

                    packet.WriteBoolean(false);// TODO: Figure out
                    packet.WriteString("");//previewImage -> e.g; catalogue/pet_lion.png
                }
            }
            else
                packet.WriteInteger(0);
            packet.WriteInteger(-1);
            packet.WriteBoolean(false);

            packet.WriteInteger(PlusEnvironment.GetGame().GetCatalog().GetPromotions().ToList().Count);//Count
            foreach (CatalogPromotion Promotion in PlusEnvironment.GetGame().GetCatalog().GetPromotions().ToList())
            {
                packet.WriteInteger(Promotion.Id);
                packet.WriteString(Promotion.Title);
                packet.WriteString(Promotion.Image);
                packet.WriteInteger(Promotion.Unknown);
                packet.WriteString(Promotion.PageLink);
                packet.WriteInteger(Promotion.ParentId);
            }
        }
    }
}