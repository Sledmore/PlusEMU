using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
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

            int pageId = packet.PopInt();
            int itemId = packet.PopInt();
            string extraData = packet.PopString();
            int amount = packet.PopInt();

            if (!PlusEnvironment.GetGame().GetCatalog().TryGetPage(pageId, out CatalogPage page))
                return;

            if (!page.Enabled || !page.Visible || page.MinimumRank > session.GetHabbo().Rank || (page.MinimumVIP > session.GetHabbo().VIPRank && session.GetHabbo().Rank == 1))
                return;

            if (!page.Items.TryGetValue(itemId, out CatalogItem item))
            {
                if (page.ItemOffers.ContainsKey(itemId))
                {
                    item = page.ItemOffers[itemId];
                    if (item == null)
                        return;
                }
                else
                    return;
            }

            if (amount < 1 || amount > 100 || !item.HaveOffer)
                amount = 1;

            int amountPurchase = item.Amount > 1 ? item.Amount : amount;

            int totalCreditsCost = amount > 1 ? ((item.CostCredits * amount) - ((int)Math.Floor((double)amount / 6) * item.CostCredits)) : item.CostCredits;
            int totalPixelCost = amount > 1 ? ((item.CostPixels * amount) - ((int)Math.Floor((double)amount / 6) * item.CostPixels)) : item.CostPixels;
            int totalDiamondCost = amount > 1 ? ((item.CostDiamonds * amount) - ((int)Math.Floor((double)amount / 6) * item.CostDiamonds)) : item.CostDiamonds;

            if (session.GetHabbo().Credits < totalCreditsCost || session.GetHabbo().Duckets < totalPixelCost || session.GetHabbo().Diamonds < totalDiamondCost)
                return;

            int limitedEditionSells = 0;
            int limitedEditionStack = 0;

            #region Create the extradata
                switch (item.Data.InteractionType)
            {
                case InteractionType.NONE:
                    extraData = "";
                    break;

                case InteractionType.GUILD_ITEM:
                case InteractionType.GUILD_GATE:
                    break;

                #region Pet handling

                case InteractionType.PET:
                    try
                    {
                        string[] bits = extraData.Split('\n');
                        string petName = bits[0];
                        string race = bits[1];
                        string color = bits[2];

                        if (!PetUtility.CheckPetName(petName))
                            return;

                        if (race.Length > 2)
                            return;

                        if (color.Length != 6)
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

                    double number = 0;

                    try
                    {
                        number = string.IsNullOrEmpty(extraData) ? 0 : double.Parse(extraData, PlusEnvironment.CultureInfo);
                    }
                    catch (Exception e)
                    {
                        ExceptionLogger.LogException(e);
                    }

                    extraData = number.ToString(CultureInfo.CurrentCulture).Replace(',', '.');
                    break; // maintain extra data // todo: validate

                case InteractionType.POSTIT:
                    extraData = "FFFF33";
                    break;

                case InteractionType.MOODLIGHT:
                    extraData = "1,1,1,#000000,255";
                    break;

                case InteractionType.TROPHY:
                    extraData = session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + Convert.ToChar(9) + extraData;
                    break;

                case InteractionType.MANNEQUIN:
                    extraData = "m" + Convert.ToChar(5) + ".ch-210-1321.lg-285-92" + Convert.ToChar(5) + "Default Mannequin";
                    break;

                case InteractionType.BADGE_DISPLAY:
                    if (!session.GetHabbo().GetBadgeComponent().HasBadge(extraData))
                    {
                        session.SendPacket(new BroadcastMessageAlertComposer("Oops, it appears that you do not own this badge."));
                        return;
                    }

                    extraData = extraData + Convert.ToChar(9) + session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                    break;

                case InteractionType.BADGE:
                    {
                        if (session.GetHabbo().GetBadgeComponent().HasBadge(item.Data.ItemName))
                        {
                            session.SendPacket(new PurchaseErrorComposer(1));
                            return;
                        }
                        break;
                    }
                default:
                    extraData = "";
                    break;
            }
            #endregion


            if (item.IsLimited)
            {
                if (item.LimitedEditionStack <= item.LimitedEditionSells)
                {
                    session.SendNotification("This item has sold out!\n\n" + "Please note, you have not recieved another item (You have also not been charged for it!)");
                    session.SendPacket(new CatalogUpdatedComposer());
                    session.SendPacket(new PurchaseOKComposer());
                    return;
                }

                item.LimitedEditionSells++;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `catalog_items` SET `limited_sells` = @limitSells WHERE `id` = @itemId LIMIT 1");
                    dbClient.AddParameter("limitSells", item.LimitedEditionSells);
                    dbClient.AddParameter("itemId", item.Id);
                    dbClient.RunQuery();

                    limitedEditionSells = item.LimitedEditionSells;
                    limitedEditionStack = item.LimitedEditionStack;
                }
            }

            if (item.CostCredits > 0)
            {
                session.GetHabbo().Credits -= totalCreditsCost;
                session.SendPacket(new CreditBalanceComposer(session.GetHabbo().Credits));
            }

            if (item.CostPixels > 0)
            {
                session.GetHabbo().Duckets -= totalPixelCost;
                session.SendPacket(new HabboActivityPointNotificationComposer(session.GetHabbo().Duckets, session.GetHabbo().Duckets));//Love you, Tom.
            }

            if (item.CostDiamonds > 0)
            {
                session.GetHabbo().Diamonds -= totalDiamondCost;
                session.SendPacket(new HabboActivityPointNotificationComposer(session.GetHabbo().Diamonds, 0, 5));
            }

            switch (item.Data.Type.ToString().ToLower())
            {
                default:
                    List<Item> generatedGenericItems = new List<Item>();

                    Item newItem;
                    switch (item.Data.InteractionType)
                    {
                        default:
                            if (amountPurchase > 1)
                            {
                                List<Item> items = ItemFactory.CreateMultipleItems(item.Data, session.GetHabbo(), extraData, amountPurchase);

                                if (items != null)
                                {
                                    generatedGenericItems.AddRange(items);
                                }
                            }
                            else
                            {
                                newItem = ItemFactory.CreateSingleItemNullable(item.Data, session.GetHabbo(), extraData, extraData, 0, limitedEditionSells, limitedEditionStack);

                                if (newItem != null)
                                {
                                    generatedGenericItems.Add(newItem);
                                }
                            }
                            break;

                        case InteractionType.GUILD_GATE:
                        case InteractionType.GUILD_ITEM:
                        case InteractionType.GUILD_FORUM:
                            if (amountPurchase > 1)
                            {
                                List<Item> items = ItemFactory.CreateMultipleItems(item.Data, session.GetHabbo(), extraData, amountPurchase, Convert.ToInt32(extraData));

                                if (items != null)
                                {
                                    generatedGenericItems.AddRange(items);
                                }
                            }
                            else
                            {
                                newItem = ItemFactory.CreateSingleItemNullable(item.Data, session.GetHabbo(), extraData, extraData, Convert.ToInt32(extraData));

                                if (newItem != null)
                                {
                                    generatedGenericItems.Add(newItem);
                                }
                            }
                            break;

                        case InteractionType.ARROW:
                        case InteractionType.TELEPORT:
                            for (int i = 0; i < amountPurchase; i++)
                            {
                                List<Item> teleItems = ItemFactory.CreateTeleporterItems(item.Data, session.GetHabbo());

                                if (teleItems != null)
                                {
                                    generatedGenericItems.AddRange(teleItems);
                                }
                            }
                            break;

                        case InteractionType.MOODLIGHT:
                            {
                                if (amountPurchase > 1)
                                {
                                    List<Item> items = ItemFactory.CreateMultipleItems(item.Data, session.GetHabbo(), extraData, amountPurchase);

                                    if (items != null)
                                    {
                                        generatedGenericItems.AddRange(items);
                                        foreach (Item I in items)
                                        {
                                            ItemFactory.CreateMoodlightData(I);
                                        }
                                    }
                                }
                                else
                                {
                                    newItem = ItemFactory.CreateSingleItemNullable(item.Data, session.GetHabbo(), extraData, extraData);

                                    if (newItem != null)
                                    {
                                        generatedGenericItems.Add(newItem);
                                        ItemFactory.CreateMoodlightData(newItem);
                                    }
                                }
                            }
                            break;

                        case InteractionType.TONER:
                            {
                                if (amountPurchase > 1)
                                {
                                    List<Item> items = ItemFactory.CreateMultipleItems(item.Data, session.GetHabbo(), extraData, amountPurchase);

                                    if (items != null)
                                    {
                                        generatedGenericItems.AddRange(items);
                                        foreach (Item I in items)
                                        {
                                            ItemFactory.CreateTonerData(I);
                                        }
                                    }
                                }
                                else
                                {
                                    newItem = ItemFactory.CreateSingleItemNullable(item.Data, session.GetHabbo(), extraData, extraData);

                                    if (newItem != null)
                                    {
                                        generatedGenericItems.Add(newItem);
                                        ItemFactory.CreateTonerData(newItem);
                                    }
                                }
                            }
                            break;

                        case InteractionType.DEAL:
                            {
                                if (PlusEnvironment.GetGame().GetCatalog().TryGetDeal(item.Data.BehaviourData, out CatalogDeal deal))
                                {
                                    foreach (CatalogItem catalogItem in deal.ItemDataList.ToList())
                                    {
                                        List<Item> items = ItemFactory.CreateMultipleItems(catalogItem.Data, session.GetHabbo(), "", amountPurchase);

                                        if (items != null)
                                        {
                                            generatedGenericItems.AddRange(items);
                                        }
                                    }
                                }
                                break;
                            }
                    }

                    foreach (Item purchasedItem in generatedGenericItems)
                    {
                        if (session.GetHabbo().GetInventoryComponent().TryAddItem(purchasedItem))
                        {
                            //Session.SendMessage(new FurniListAddComposer(PurchasedItem));
                            session.SendPacket(new FurniListNotificationComposer(purchasedItem.Id, 1));
                        }
                    }
                    break;

                case "e":
                    AvatarEffect effect;

                    if (session.GetHabbo().Effects().HasEffect(item.Data.SpriteId))
                    {
                        effect = session.GetHabbo().Effects().GetEffectNullable(item.Data.SpriteId);

                        if (effect != null)
                        {
                            effect.AddToQuantity();
                        }
                    }
                    else
                        effect = AvatarEffectFactory.CreateNullable(session.GetHabbo(), item.Data.SpriteId, 3600);

                    if (effect != null)// && Session.GetHabbo().Effects().TryAdd(Effect))
                    {
                        session.SendPacket(new AvatarEffectAddedComposer(item.Data.SpriteId, 3600));
                    }
                    break;

                case "r":
                    Bot bot = BotUtility.CreateBot(item.Data, session.GetHabbo().Id);
                    if (bot != null)
                    {
                        session.GetHabbo().GetInventoryComponent().TryAddBot(bot);
                        session.SendPacket(new BotInventoryComposer(session.GetHabbo().GetInventoryComponent().GetBots()));
                        session.SendPacket(new FurniListNotificationComposer(bot.Id, 5));
                    }
                    else
                        session.SendNotification("Oops! There was an error whilst purchasing this bot. It seems that there is no bot data for the bot!");
                    break;

                case "b":
                    {
                        session.GetHabbo().GetBadgeComponent().GiveBadge(item.Data.ItemName, true, session);
                        session.SendPacket(new FurniListNotificationComposer(0, 4));
                        break;
                    }

                case "p":
                    {
                        string[] petData = extraData.Split('\n');

                        Pet pet = PetUtility.CreatePet(session.GetHabbo().Id, petData[0], item.Data.BehaviourData, petData[1], petData[2]);
                        if (pet != null)
                        {
                            if (session.GetHabbo().GetInventoryComponent().TryAddPet(pet))
                            {
                                pet.RoomId = 0;
                                pet.PlacedInRoom = false;

                                session.SendPacket(new FurniListNotificationComposer(pet.PetId, 3));
                                session.SendPacket(new PetInventoryComposer(session.GetHabbo().GetInventoryComponent().GetPets()));

                                if (PlusEnvironment.GetGame().GetItemManager().GetItem(320, out ItemData petFood))
                                {
                                    Item food = ItemFactory.CreateSingleItemNullable(petFood, session.GetHabbo(), "", "");
                                    if (food != null)
                                    {
                                        session.GetHabbo().GetInventoryComponent().TryAddItem(food);
                                        session.SendPacket(new FurniListNotificationComposer(food.Id, 1));
                                    }
                                }
                            }
                        }
                        break;
                    }
            }


            if (!string.IsNullOrEmpty(item.Badge) &&
                PlusEnvironment.GetGame().GetBadgeManager().TryGetBadge(item.Badge, out BadgeDefinition badge) &&
                (string.IsNullOrEmpty(badge.RequiredRight) || session.GetHabbo().GetPermissions().HasRight(badge.RequiredRight)))
            {
                session.GetHabbo().GetBadgeComponent().GiveBadge(badge.Code, true, session);
            }

            session.SendPacket(new PurchaseOKComposer(item, item.Data));
            session.SendPacket(new FurniListUpdateComposer());
        }
    }
}