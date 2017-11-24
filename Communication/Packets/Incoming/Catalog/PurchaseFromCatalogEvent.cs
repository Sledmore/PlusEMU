using System;
using System.Linq;
using System.Collections.Generic;

using Plus.Core;
using Plus.HabboHotel.Catalog;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms;
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
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (PlusEnvironment.GetSettingsManager().TryGetValue("catalog.enabled") != "1")
            {
                Session.SendNotification("The hotel managers have disabled the catalogue");
                return;
            }

            int PageId = Packet.PopInt();
            int ItemId = Packet.PopInt();
            string ExtraData = Packet.PopString();
            int Amount = Packet.PopInt();


            CatalogPage Page = null;
            if (!PlusEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out Page))
                return;

            if (!Page.Enabled || !Page.Visible || Page.MinimumRank > Session.GetHabbo().Rank || (Page.MinimumVIP > Session.GetHabbo().VIPRank && Session.GetHabbo().Rank == 1))
                return;

            CatalogItem Item = null;
            if (!Page.Items.TryGetValue(ItemId, out Item))
            {
                if (Page.ItemOffers.ContainsKey(ItemId))
                {
                    Item = (CatalogItem)Page.ItemOffers[ItemId];
                    if (Item == null)
                        return;
                }
                else
                    return;
            }

            if (Amount < 1 || Amount > 100 || !Item.HaveOffer)
                Amount = 1;

            int AmountPurchase = Item.Amount > 1 ? Item.Amount : Amount;

            int TotalCreditsCost = Amount > 1 ? ((Item.CostCredits * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostCredits)) : Item.CostCredits;
            int TotalPixelCost = Amount > 1 ? ((Item.CostPixels * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostPixels)) : Item.CostPixels;
            int TotalDiamondCost = Amount > 1 ? ((Item.CostDiamonds * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostDiamonds)) : Item.CostDiamonds;

            if (Session.GetHabbo().Credits < TotalCreditsCost || Session.GetHabbo().Duckets < TotalPixelCost || Session.GetHabbo().Diamonds < TotalDiamondCost)
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

                        PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_PetLover", 1);
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

                    Double Number = 0;

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
                    ExtraData = Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + Convert.ToChar(9) + ExtraData;
                    break;

                case InteractionType.MANNEQUIN:
                    ExtraData = "m" + Convert.ToChar(5) + ".ch-210-1321.lg-285-92" + Convert.ToChar(5) + "Default Mannequin";
                    break;

                case InteractionType.BADGE_DISPLAY:
                    if (!Session.GetHabbo().GetBadgeComponent().HasBadge(ExtraData))
                    {
                        Session.SendPacket(new BroadcastMessageAlertComposer("Oops, it appears that you do not own this badge."));
                        return;
                    }

                    ExtraData = ExtraData + Convert.ToChar(9) + Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                    break;

                case InteractionType.BADGE:
                    {
                        if (Session.GetHabbo().GetBadgeComponent().HasBadge(Item.Data.ItemName))
                        {
                            Session.SendPacket(new PurchaseErrorComposer(1));
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
                    Session.SendNotification("This item has sold out!\n\n" + "Please note, you have not recieved another item (You have also not been charged for it!)");
                    Session.SendPacket(new CatalogUpdatedComposer());
                    Session.SendPacket(new PurchaseOKComposer());
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
                Session.GetHabbo().Credits -= TotalCreditsCost;
                Session.SendPacket(new CreditBalanceComposer(Session.GetHabbo().Credits));
            }

            if (Item.CostPixels > 0)
            {
                Session.GetHabbo().Duckets -= TotalPixelCost;
                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));//Love you, Tom.
            }

            if (Item.CostDiamonds > 0)
            {
                Session.GetHabbo().Diamonds -= TotalDiamondCost;
                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
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
                                List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), ExtraData, AmountPurchase);

                                if (Items != null)
                                {
                                    GeneratedGenericItems.AddRange(Items);
                                }
                            }
                            else
                            {
                                NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), ExtraData, ExtraData, 0, LimitedEditionSells, LimitedEditionStack);

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
                                List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), ExtraData, AmountPurchase, Convert.ToInt32(ExtraData));

                                if (Items != null)
                                {
                                    GeneratedGenericItems.AddRange(Items);
                                }
                            }
                            else
                            {
                                NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), ExtraData, ExtraData, Convert.ToInt32(ExtraData));

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
                                List<Item> TeleItems = ItemFactory.CreateTeleporterItems(Item.Data, Session.GetHabbo());

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
                                    List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), ExtraData, AmountPurchase);

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
                                    NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), ExtraData, ExtraData);

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
                                    List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), ExtraData, AmountPurchase);

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
                                    NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), ExtraData, ExtraData);

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
                                CatalogDeal deal = null;
                                if (PlusEnvironment.GetGame().GetCatalog().TryGetDeal(Item.Data.BehaviourData, out deal))
                                {
                                    foreach (CatalogItem CatalogItem in deal.ItemDataList.ToList())
                                    {
                                        List<Item> Items = ItemFactory.CreateMultipleItems(CatalogItem.Data, Session.GetHabbo(), "", AmountPurchase);

                                        if (Items != null)
                                        {
                                            GeneratedGenericItems.AddRange(Items);
                                        }
                                    }
                                }
                                break;
                            }

                        case InteractionType.ROOMDEAL:
                            {
                                CatalogDeal deal = null;
                                if (PlusEnvironment.GetGame().GetCatalog().TryGetDeal(Item.Data.BehaviourData, out deal))
                                {
                                    Room room = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(deal.RoomId);
                                    if (room == null)
                                        Session.SendNotification("There was an error loading this Room Bundle, if this happens again please contact hotel management!");

                                    RoomData newRoom = PlusEnvironment.GetGame().GetRoomManager().CreateRoom(Session, room.Name, room.Description, room.ModelName, room.Category, 10, room.TradeSettings, room.Wallpaper, room.Floor, room.Landscape, room.WallThickness, room.FloorThickness);
                                    if (newRoom == null)
                                        return;

                                    Room myRoom = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(newRoom.Id);
                                    if (myRoom != null)
                                    {
                                        List<Item> Items = ItemLoader.GetItemsForRoom(deal.RoomId, room);
                                        Item teleLink = null;

                                        foreach (var furni in Items)
                                        {
                                            if (furni.Data.InteractionType == InteractionType.TELEPORT)
                                            {
                                                if (teleLink == null)
                                                {
                                                    List<Item> TeleItems = ItemFactory.CreateTeleporterItems(furni.Data, Session.GetHabbo());

                                                    if (TeleItems != null && TeleItems.ToList().Count > 1)
                                                    {
                                                        myRoom.GetRoomItemHandler().SetFloorItem(Session, TeleItems[0], furni.GetX, furni.GetY, furni.Rotation, true, true, true, false, furni.GetZ);
                                                        teleLink = TeleItems[1];
                                                    }
                                                }
                                                else
                                                {
                                                    myRoom.GetRoomItemHandler().SetFloorItem(Session, teleLink, furni.GetX, furni.GetY, furni.Rotation, true, true, true, false, furni.GetZ);
                                                    teleLink = null;
                                                }
                                            }
                                            else
                                            {
                                                NewItem = ItemFactory.CreateSingleItemNullable(furni.Data, Session.GetHabbo(), furni.ExtraData, "", furni.GroupId);

                                                if (NewItem != null)
                                                {
                                                    if (NewItem.IsWallItem)
                                                    {
                                                        NewItem.wallCoord = furni.wallCoord;
                                                        myRoom.GetRoomItemHandler().SetWallItem(Session, NewItem);
                                                    }
                                                    else
                                                        myRoom.GetRoomItemHandler().SetFloorItem(Session, NewItem, furni.GetX, furni.GetY, furni.Rotation, true, true, true, false, furni.GetZ);
                                                }
                                            }
                                        }
                                        Session.GetHabbo().PrepareRoom(myRoom.Id, "");
                                    }
                                }
                                break;
                            }

                    }

                    foreach (Item PurchasedItem in GeneratedGenericItems)
                    {
                        if (Session.GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                        {
                            //Session.SendMessage(new FurniListAddComposer(PurchasedItem));
                            Session.SendPacket(new FurniListNotificationComposer(PurchasedItem.Id, 1));
                        }
                    }
                    break;

                case "e":
                    AvatarEffect Effect = null;

                    if (Session.GetHabbo().Effects().HasEffect(Item.Data.SpriteId))
                    {
                        Effect = Session.GetHabbo().Effects().GetEffectNullable(Item.Data.SpriteId);

                        if (Effect != null)
                        {
                            Effect.AddToQuantity();
                        }
                    }
                    else
                        Effect = AvatarEffectFactory.CreateNullable(Session.GetHabbo(), Item.Data.SpriteId, 3600);

                    if (Effect != null)// && Session.GetHabbo().Effects().TryAdd(Effect))
                    {
                        Session.SendPacket(new AvatarEffectAddedComposer(Item.Data.SpriteId, 3600));
                    }
                    break;

                case "r":
                    Bot Bot = BotUtility.CreateBot(Item.Data, Session.GetHabbo().Id);
                    if (Bot != null)
                    {
                        Session.GetHabbo().GetInventoryComponent().TryAddBot(Bot);
                        Session.SendPacket(new BotInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetBots()));
                        Session.SendPacket(new FurniListNotificationComposer(Bot.Id, 5));
                    }
                    else
                        Session.SendNotification("Oops! There was an error whilst purchasing this bot. It seems that there is no bot data for the bot!");
                    break;

                case "b":
                    {
                        Session.GetHabbo().GetBadgeComponent().GiveBadge(Item.Data.ItemName, true, Session);
                        Session.SendPacket(new FurniListNotificationComposer(0, 4));
                        break;
                    }

                case "p":
                    {
                        string[] PetData = ExtraData.Split('\n');

                        Pet GeneratedPet = PetUtility.CreatePet(Session.GetHabbo().Id, PetData[0], Item.Data.BehaviourData, PetData[1], PetData[2]);
                        if (GeneratedPet != null)
                        {
                            Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet);

                            Session.SendPacket(new FurniListNotificationComposer(GeneratedPet.PetId, 3));
                            Session.SendPacket(new PetInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetPets()));

                            ItemData PetFood = null;
                            if (PlusEnvironment.GetGame().GetItemManager().GetItem(320, out PetFood))
                            {
                                Item Food = ItemFactory.CreateSingleItemNullable(PetFood, Session.GetHabbo(), "", "");
                                if (Food != null)
                                {
                                    Session.GetHabbo().GetInventoryComponent().TryAddItem(Food);
                                    Session.SendPacket(new FurniListNotificationComposer(Food.Id, 1));
                                }
                            }
                        }
                        break;
                    }
            }

            BadgeDefinition Badge;

            if(!string.IsNullOrEmpty(Item.Badge) &&
                PlusEnvironment.GetGame().GetBadgeManager().TryGetBadge(Item.Badge, out Badge) &&
                (string.IsNullOrEmpty(Badge.RequiredRight) || Session.GetHabbo().GetPermissions().HasRight(Badge.RequiredRight)))
            {
                Session.GetHabbo().GetBadgeComponent().GiveBadge(Badge.Code, true, Session);
            }

            Session.SendPacket(new PurchaseOKComposer(Item, Item.Data));
            Session.SendPacket(new FurniListUpdateComposer());
        }
    }
}