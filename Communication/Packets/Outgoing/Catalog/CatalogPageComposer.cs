using System.Linq;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Catalog;
using Plus.HabboHotel.Catalog.Utilities;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class CatalogPageComposer : ServerPacket
    {
        public CatalogPageComposer(CatalogPage Page, string CataMode)
            : base(ServerPacketHeader.CatalogPageMessageComposer)
        {
            WriteInteger(Page.Id);
            WriteString(CataMode);
            WriteString(Page.Template);

            WriteInteger(Page.PageStrings1.Count);
            foreach (string s in Page.PageStrings1)
            {
                WriteString(s);
            }

            WriteInteger(Page.PageStrings2.Count);
            foreach (string s in Page.PageStrings2)
            {
                WriteString(s);
            }

            if (!Page.Template.Equals("frontpage") && !Page.Template.Equals("club_buy"))
            {
                WriteInteger(Page.Items.Count);
                foreach (CatalogItem Item in Page.Items.Values)
                {
                    WriteInteger(Item.Id);
                    WriteString(Item.Name);
                    WriteBoolean(false);//IsRentable
                    WriteInteger(Item.CostCredits);

                    if (Item.CostDiamonds > 0)
                    {
                        WriteInteger(Item.CostDiamonds);
                        WriteInteger(5); // Diamonds
                    }
                    else
                    {
                        WriteInteger(Item.CostPixels);
                        WriteInteger(0); // Type of PixelCost
                    }

                    WriteBoolean(ItemUtility.CanGiftItem(Item));

                    if (Item.Data.InteractionType == InteractionType.DEAL || Item.Data.InteractionType == InteractionType.ROOMDEAL)
                    {
                        CatalogDeal deal = null;
                        if (!PlusEnvironment.GetGame().GetCatalog().TryGetDeal(Item.Data.BehaviourData, out deal))
                        {
                            WriteInteger(0);//Count
                        }
                        else
                        {
                            WriteInteger(deal.ItemDataList.Count);
                            
                            foreach (CatalogItem dealItem in deal.ItemDataList.ToList())
                            {
                                WriteString(dealItem.Data.Type.ToString());
                                WriteInteger(dealItem.Data.SpriteId);
                                WriteString("");
                                WriteInteger(dealItem.Amount);
                                WriteBoolean(false);
                            }
                        }
                    }
                    else
                    {
                        WriteInteger(string.IsNullOrEmpty(Item.Badge) ? 1 : 2);//Count 1 item if there is no badge, otherwise count as 2.
                        
                        if (!string.IsNullOrEmpty(Item.Badge))
                        {
                            WriteString("b");
                            WriteString(Item.Badge);
                        }

                        WriteString(Item.Data.Type.ToString());
                        if (Item.Data.Type.ToString().ToLower() == "b")
                        {
                            //This is just a badge, append the name.
                            WriteString(Item.Data.ItemName);
                        }
                        else
                        {
                            WriteInteger(Item.Data.SpriteId);
                            if (Item.Data.InteractionType == InteractionType.WALLPAPER || Item.Data.InteractionType == InteractionType.FLOOR || Item.Data.InteractionType == InteractionType.LANDSCAPE)
                            {
                                WriteString(Item.Name.Split('_')[2]);
                            }
                            else if (Item.Data.InteractionType == InteractionType.BOT)//Bots
                            {
                                CatalogBot CatalogBot = null;
                                if (!PlusEnvironment.GetGame().GetCatalog().TryGetBot(Item.ItemId, out CatalogBot))
                                    WriteString("hd-180-7.ea-1406-62.ch-210-1321.hr-831-49.ca-1813-62.sh-295-1321.lg-285-92");
                                else
                                    WriteString(CatalogBot.Figure);
                            }
                            else if (Item.ExtraData != null)
                            {
                                WriteString(Item.ExtraData != null ? Item.ExtraData : string.Empty);
                            }
                            WriteInteger(Item.Amount);
                            WriteBoolean(Item.IsLimited); // IsLimited
                            if (Item.IsLimited)
                            {
                                WriteInteger(Item.LimitedEditionStack);
                                WriteInteger(Item.LimitedEditionStack - Item.LimitedEditionSells);
                            }
                        }
                    }
                    WriteInteger(0); //club_level
                    WriteBoolean(ItemUtility.CanSelectAmount(Item));

                    WriteBoolean(false);// TODO: Figure out
                    WriteString("");//previewImage -> e.g; catalogue/pet_lion.png
                }
            }
            else
                WriteInteger(0);
            WriteInteger(-1);
            WriteBoolean(false);

            WriteInteger(PlusEnvironment.GetGame().GetCatalog().GetPromotions().ToList().Count);//Count
            foreach (CatalogPromotion Promotion in PlusEnvironment.GetGame().GetCatalog().GetPromotions().ToList())
            {
                WriteInteger(Promotion.Id);
                WriteString(Promotion.Title);
                WriteString(Promotion.Image);
                WriteInteger(Promotion.Unknown);
                WriteString(Promotion.PageLink);
                WriteInteger(Promotion.ParentId);
            }
        }
    }
}