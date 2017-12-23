using System;
using System.Linq;
using System.Collections.Generic;

using Plus.Core;
using Plus.HabboHotel.Catalog;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Users.Effects;
using Plus.HabboHotel.Users.Inventory.Bots;

using Plus.HabboHotel.Rooms.AI;
using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.Communication.Packets.Outgoing.Inventory.Bots;
using Plus.Communication.Packets.Outgoing.Inventory.Pets;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.Communication.Packets.Outgoing.Inventory.Furni;
using Plus.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.Catalog.Utilities;
using Plus.HabboHotel.Badges;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    public class PurchaseFromCatalogEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (PlusEnvironment.GetSettingsManager().TryGetValue("catalog.enabled") != "1")
            {
                session.SendNotification("The hotel managers have disabled the catalogue");
                return;
            }

            int PageId = packet.PopInt();
            int ItemId = packet.PopInt();
            string ExtraData = packet.PopString();
            int amount = packet.PopInt();

            if (!PlusEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out CatalogPage page))
                return;

            if (!page.Enabled || !page.Visible || page.MinimumRank > session.GetHabbo().Rank || (page.MinimumVIP > session.GetHabbo().VIPRank && session.GetHabbo().Rank == 1))
                return;

            if (!page.Items.TryGetValue(ItemId, out CatalogItem Item))
            {
                if (page.ItemOffers.ContainsKey(ItemId))
                {
                    Item = page.ItemOffers[ItemId];
                    if (Item == null)
                        return;
                }
                else
                    return;
            }

            if (amount < 1 || amount > 100 || !Item.HaveOffer)
                amount = 1;

            int AmountPurchase = Item.Amount > 1 ? Item.Amount : amount;

            int TotalCreditsCost = amount > 1 ? ((Item.CostCredits * amount) - ((int)Math.Floor((double)amount / 6) * Item.CostCredits)) : Item.CostCredits;
            int TotalPixelCost = amount > 1 ? ((Item.CostPixels * amount) - ((int)Math.Floor((double)amount / 6) * Item.CostPixels)) : Item.CostPixels;
            int TotalDiamondCost = amount > 1 ? ((Item.CostDiamonds * amount) - ((int)Math.Floor((double)amount / 6) * Item.CostDiamonds)) : Item.CostDiamonds;

            if (session.GetHabbo().Credits < TotalCreditsCost || session.GetHabbo().Duckets < TotalPixelCost || session.GetHabbo().Diamonds < TotalDiamondCost)
                return;

            int LimitedEditionSells = 0;
            int LimitedEditionStack = 0;

            #region Create the extradata
                switch (Item.Data.InteractionType)
            {
                case InteractionType.NONE:
                    ExtraData = "";
                    break;

                case InteractionType.GUILD_ITEM:
                case InteractionType.GUILD_GATE:
                    break;

                #region Pet handling

                case InteractionType.PET:
                    try
                    {
                        string[] Bits = ExtraData.Split('\n');
                        string PetName = Bits[0];
                        string Race = Bits[1];
                        string Color = Bits[2];

                        int.Parse(Race); // to trigger any possible errors

                        if (!PetUtility.CheckPetName(PetName))
                            return;

                        if (Race.Length > 2)
                            return;

                        if (Color.Length != 6)
                            return;

                        PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_PetLover", 1);
                    }
                    catch (Exception e)
                    {
                        ExceptionLogger.LogException(e);
                        return;
                    }

                    break;

                #endregion

                case InteractionType.FLOOR:
                case InteractionType.WALLPAPER:
                case InteractionType.LANDSCAPE:

                    double Number = 0;

                    try
                    {
                        if (string.IsNullOrEmpty(ExtraData))
                            Number = 0;
                        else
                            Number = Double.Parse(ExtraData, PlusEnvironment.CultureInfo);
                    }
                    catch (Exception e)
                    {
                        ExceptionLogger.LogException(e);
                    }

                    ExtraData = Number.ToString().Replace(',', '.');
                    break; // maintain extra data // todo: validate

                case InteractionType.POSTIT:
                    ExtraData = "FFFF33";
                    break;

                case InteractionType.MOODLIGHT:
                    ExtraData = "1,1,1,#000000,255";
                    break;

                case InteractionType.TROPHY:
                    ExtraData = session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + Convert.ToChar(9) + ExtraData;
                    break;

                case InteractionType.MANNEQUIN:
                    ExtraData = "m" + Convert.ToChar(5) + ".ch-210-1321.lg-285-92" + Convert.ToChar(5) + "Default Mannequin";
                    break;

                case InteractionType.BADGE_DISPLAY:
                    if (!session.GetHabbo().GetBadgeComponent().HasBadge(ExtraData))
                    {
                        session.SendPacket(new BroadcastMessageAlertComposer("Oops, it appears that you do not own this badge."));
                        return;
                    }

                    ExtraData = ExtraData + Convert.ToChar(9) + session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                    break;

                case InteractionType.BADGE:
                    {
                        if (session.GetHabbo().GetBadgeComponent().HasBadge(Item.Data.ItemName))
                        {
                            session.SendPacket(new PurchaseErrorComposer(1));
                            return;
                        }
                        break;
                    }
                default:
                    ExtraData = "";
                    break;
            }
            #endregion


            if (Item.IsLimited)
            {
                if (Item.LimitedEditionStack <= Item.LimitedEditionSells)
                {
                    session.SendNotification("This item has sold out!\n\n" + "Please note, you have not recieved another item (You have also not been charged for it!)");
                    session.SendPacket(new CatalogUpdatedComposer());
                    session.SendPacket(new PurchaseOKComposer());
                    return;
                }

                Item.LimitedEditionSells++;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `catalog_items` SET `limited_sells` = @limitSells WHERE `id` = @itemId LIMIT 1");
                    dbClient.AddParameter("limitSells", Item.LimitedEditionSells);
                    dbClient.AddParameter("itemId", Item.Id);
                    dbClient.RunQuery();

                    LimitedEditionSells = Item.LimitedEditionSells;
                    LimitedEditionStack = Item.LimitedEditionStack;
                }
            }

            if (Item.CostCredits > 0)
            {
                session.GetHabbo().Credits -= TotalCreditsCost;
                session.SendPacket(new CreditBalanceComposer(session.GetHabbo().Credits));
            }

            if (Item.CostPixels > 0)
            {
                session.GetHabbo().Duckets -= TotalPixelCost;
                session.SendPacket(new HabboActivityPointNotificationComposer(session.GetHabbo().Duckets, session.GetHabbo().Duckets));//Love you, Tom.
            }

            if (Item.CostDiamonds > 0)
            {
                session.GetHabbo().Diamonds -= TotalDiamondCost;
                session.SendPacket(new HabboActivityPointNotificationComposer(session.GetHabbo().Diamonds, 0, 5));
            }

            Item NewItem = null;
            switch (Item.Data.Type.ToString().ToLower())
            {
                default:
                    List<Item> GeneratedGenericItems = new List<Item>();

                    switch (Item.Data.InteractionType)
                    {
                        default:
                            if (AmountPurchase > 1)
                            {
                                List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, session.GetHabbo(), ExtraData, AmountPurchase);

                                if (Items != null)
                                {
                                    GeneratedGenericItems.AddRange(Items);
                                }
                            }
                            else
                            {
                                NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, session.GetHabbo(), ExtraData, ExtraData, 0, LimitedEditionSells, LimitedEditionStack);

                                if (NewItem != null)
                                {
                                    GeneratedGenericItems.Add(NewItem);
                                }
                            }
                            break;

                        case InteractionType.GUILD_GATE:
                        case InteractionType.GUILD_ITEM:
                        case InteractionType.GUILD_FORUM:
                            if (AmountPurchase > 1)
                            {
                                List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, session.GetHabbo(), ExtraData, AmountPurchase, Convert.ToInt32(ExtraData));

                                if (Items != null)
                                {
                                    GeneratedGenericItems.AddRange(Items);
                                }
                            }
                            else
                            {
                                NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, session.GetHabbo(), ExtraData, ExtraData, Convert.ToInt32(ExtraData));

                                if (NewItem != null)
                                {
                                    GeneratedGenericItems.Add(NewItem);
                                }
                            }
                            break;

                        case InteractionType.ARROW:
                        case InteractionType.TELEPORT:
                            for (int i = 0; i < AmountPurchase; i++)
                            {
                                List<Item> TeleItems = ItemFactory.CreateTeleporterItems(Item.Data, session.GetHabbo());

                                if (TeleItems != null)
                                {
                                    GeneratedGenericItems.AddRange(TeleItems);
                                }
                            }
                            break;

                        case InteractionType.MOODLIGHT:
                            {
                                if (AmountPurchase > 1)
                                {
                                    List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, session.GetHabbo(), ExtraData, AmountPurchase);

                                    if (Items != null)
                                    {
                                        GeneratedGenericItems.AddRange(Items);
                                        foreach (Item I in Items)
                                        {
                                            ItemFactory.CreateMoodlightData(I);
                                        }
                                    }
                                }
                                else
                                {
                                    NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, session.GetHabbo(), ExtraData, ExtraData);

                                    if (NewItem != null)
                                    {
                                        GeneratedGenericItems.Add(NewItem);
                                        ItemFactory.CreateMoodlightData(NewItem);
                                    }
                                }
                            }
                            break;

                        case InteractionType.TONER:
                            {
                                if (AmountPurchase > 1)
                                {
                                    List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, session.GetHabbo(), ExtraData, AmountPurchase);

                                    if (Items != null)
                                    {
                                        GeneratedGenericItems.AddRange(Items);
                                        foreach (Item I in Items)
                                        {
                                            ItemFactory.CreateTonerData(I);
                                        }
                                    }
                                }
                                else
                                {
                                    NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, session.GetHabbo(), ExtraData, ExtraData);

                                    if (NewItem != null)
                                    {
                                        GeneratedGenericItems.Add(NewItem);
                                        ItemFactory.CreateTonerData(NewItem);
                                    }
                                }
                            }
                            break;

                        case InteractionType.DEAL:
                            {
                                if (PlusEnvironment.GetGame().GetCatalog().TryGetDeal(Item.Data.BehaviourData, out CatalogDeal deal))
                                {
                                    foreach (CatalogItem CatalogItem in deal.ItemDataList.ToList())
                                    {
                                        List<Item> Items = ItemFactory.CreateMultipleItems(CatalogItem.Data, session.GetHabbo(), "", AmountPurchase);

                                        if (Items != null)
                                        {
                                            GeneratedGenericItems.AddRange(Items);
                                        }
                                    }
                                }
                                break;
                            }
                    }

                    foreach (Item PurchasedItem in GeneratedGenericItems)
                    {
                        if (session.GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                        {
                            //Session.SendMessage(new FurniListAddComposer(PurchasedItem));
                            session.SendPacket(new FurniListNotificationComposer(PurchasedItem.Id, 1));
                        }
                    }
                    break;

                case "e":
                    AvatarEffect Effect = null;

                    if (session.GetHabbo().Effects().HasEffect(Item.Data.SpriteId))
                    {
                        Effect = session.GetHabbo().Effects().GetEffectNullable(Item.Data.SpriteId);

                        if (Effect != null)
                        {
                            Effect.AddToQuantity();
                        }
                    }
                    else
                        Effect = AvatarEffectFactory.CreateNullable(session.GetHabbo(), Item.Data.SpriteId, 3600);

                    if (Effect != null)// && Session.GetHabbo().Effects().TryAdd(Effect))
                    {
                        session.SendPacket(new AvatarEffectAddedComposer(Item.Data.SpriteId, 3600));
                    }
                    break;

                case "r":
                    Bot Bot = BotUtility.CreateBot(Item.Data, session.GetHabbo().Id);
                    if (Bot != null)
                    {
                        session.GetHabbo().GetInventoryComponent().TryAddBot(Bot);
                        session.SendPacket(new BotInventoryComposer(session.GetHabbo().GetInventoryComponent().GetBots()));
                        session.SendPacket(new FurniListNotificationComposer(Bot.Id, 5));
                    }
                    else
                        session.SendNotification("Oops! There was an error whilst purchasing this bot. It seems that there is no bot data for the bot!");
                    break;

                case "b":
                    {
                        session.GetHabbo().GetBadgeComponent().GiveBadge(Item.Data.ItemName, true, session);
                        session.SendPacket(new FurniListNotificationComposer(0, 4));
                        break;
                    }

                case "p":
                    {
                        string[] PetData = ExtraData.Split('\n');

                        Pet GeneratedPet = PetUtility.CreatePet(session.GetHabbo().Id, PetData[0], Item.Data.BehaviourData, PetData[1], PetData[2]);
                        if (GeneratedPet != null)
                        {
                            session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet);

                            session.SendPacket(new FurniListNotificationComposer(GeneratedPet.PetId, 3));
                            session.SendPacket(new PetInventoryComposer(session.GetHabbo().GetInventoryComponent().GetPets()));

                            if (PlusEnvironment.GetGame().GetItemManager().GetItem(320, out ItemData PetFood))
                            {
                                Item Food = ItemFactory.CreateSingleItemNullable(PetFood, session.GetHabbo(), "", "");
                                if (Food != null)
                                {
                                    session.GetHabbo().GetInventoryComponent().TryAddItem(Food);
                                    session.SendPacket(new FurniListNotificationComposer(Food.Id, 1));
                                }
                            }
                        }
                        break;
                    }
            }


            if (!string.IsNullOrEmpty(Item.Badge) &&
                PlusEnvironment.GetGame().GetBadgeManager().TryGetBadge(Item.Badge, out BadgeDefinition Badge) &&
                (string.IsNullOrEmpty(Badge.RequiredRight) || session.GetHabbo().GetPermissions().HasRight(Badge.RequiredRight)))
            {
                session.GetHabbo().GetBadgeComponent().GiveBadge(Badge.Code, true, session);
            }

            session.SendPacket(new PurchaseOKComposer(Item, Item.Data));
            session.SendPacket(new FurniListUpdateComposer());
        }
    }
}