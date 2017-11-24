using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

using Plus.HabboHotel.GameClients;

using Plus.HabboHotel.Items;
using Plus.HabboHotel.Catalog.Marketplace;
using Plus.Communication.Packets.Outgoing.Marketplace;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.Communication.Packets.Outgoing.Inventory.Furni;

using Plus.Database.Interfaces;


namespace Plus.Communication.Packets.Incoming.Marketplace
{
    class BuyOfferEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int OfferId = Packet.PopInt();

            DataRow Row = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `state`,`timestamp`,`total_price`,`extra_data`,`item_id`,`furni_id`,`user_id`,`limited_number`,`limited_stack` FROM `catalog_marketplace_offers` WHERE `offer_id` = @OfferId LIMIT 1");
                dbClient.AddParameter("OfferId", OfferId);
                Row = dbClient.GetRow();
            }

            if (Row == null)
            {
                this.ReloadOffers(Session);
                return;
            }

            if (Convert.ToString(Row["state"]) == "2")
            {
                Session.SendNotification("Oops, this offer is no longer available.");
                this.ReloadOffers(Session);
                return;
            }

            if (PlusEnvironment.GetGame().GetCatalog().GetMarketplace().FormatTimestamp() > (Convert.ToDouble(Row["timestamp"])))
            {
                Session.SendNotification("Oops, this offer has expired..");
                this.ReloadOffers(Session);
                return;
            }

            ItemData Item = null;
            if (!PlusEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(Row["item_id"]), out Item))
            {
                Session.SendNotification("Item isn't in the hotel anymore.");
                this.ReloadOffers(Session);
                return;
            }
            else
            {
                if (Convert.ToInt32(Row["user_id"]) == Session.GetHabbo().Id)
                {
                    Session.SendNotification("To prevent average boosting you cannot purchase your own marketplace offers.");
                    return;
                }

                if (Convert.ToInt32(Row["total_price"]) > Session.GetHabbo().Credits)
                {
                    Session.SendNotification("Oops, you do not have enough credits for this.");
                    return;
                }

                Session.GetHabbo().Credits -= Convert.ToInt32(Row["total_price"]);
                Session.SendPacket(new CreditBalanceComposer(Session.GetHabbo().Credits));


                Item GiveItem = ItemFactory.CreateSingleItem(Item, Session.GetHabbo(), Convert.ToString(Row["extra_data"]), Convert.ToString(Row["extra_data"]), Convert.ToInt32(Row["furni_id"]), Convert.ToInt32(Row["limited_number"]), Convert.ToInt32(Row["limited_stack"]));
                if (GiveItem != null)
                {
                    Session.GetHabbo().GetInventoryComponent().TryAddItem(GiveItem);
                    Session.SendPacket(new FurniListNotificationComposer(GiveItem.Id, 1));

                    Session.SendPacket(new Plus.Communication.Packets.Outgoing.Catalog.PurchaseOKComposer());
                    Session.SendPacket(new FurniListAddComposer(GiveItem));            
                    Session.SendPacket(new FurniListUpdateComposer());
                }


                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `catalog_marketplace_offers` SET `state` = '2' WHERE `offer_id` = '" + OfferId + "' LIMIT 1");

                    int Id = 0;
                    dbClient.SetQuery("SELECT `id` FROM `catalog_marketplace_data` WHERE `sprite` = " + Item.SpriteId + " LIMIT 1;");
                    Id = dbClient.GetInteger();

                    if (Id > 0)
                        dbClient.RunQuery("UPDATE `catalog_marketplace_data` SET `sold` = `sold` + 1, `avgprice` = (avgprice + " + Convert.ToInt32(Row["total_price"]) + ") WHERE `id` = " + Id + " LIMIT 1;");
                    else
                        dbClient.RunQuery("INSERT INTO `catalog_marketplace_data` (`sprite`, `sold`, `avgprice`) VALUES ('" + Item.SpriteId + "', '1', '" + Convert.ToInt32(Row["total_price"]) + "')");


                    if (PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.ContainsKey(Item.SpriteId) && PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.ContainsKey(Item.SpriteId))
                    {
                        int num3 = PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts[Item.SpriteId];
                        int num4 = (PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages[Item.SpriteId] += Convert.ToInt32(Row["total_price"]));

                        PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.Remove(Item.SpriteId);
                        PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.Add(Item.SpriteId, num4);
                        PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.Remove(Item.SpriteId);
                        PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.Add(Item.SpriteId, num3 + 1);
                    }
                    else
                    {
                        if (!PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.ContainsKey(Item.SpriteId))
                            PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.Add(Item.SpriteId, Convert.ToInt32(Row["total_price"]));

                        if (!PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.ContainsKey(Item.SpriteId))
                            PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.Add(Item.SpriteId, 1);
                    }
                }
            }

            this.ReloadOffers(Session);
        }


        private void ReloadOffers(GameClient Session)
        {
            int MinCost = -1;
            int MaxCost = -1;
            string SearchQuery = "";
            int FilterMode = 1;


            DataTable table = null;
            StringBuilder builder = new StringBuilder();
            string str = "";
            builder.Append("WHERE `state` = '1' AND `timestamp` >= " + PlusEnvironment.GetGame().GetCatalog().GetMarketplace().FormatTimestampString());
            if (MinCost >= 0)
            {
                builder.Append(" AND `total_price` > " + MinCost);
            }
            if (MaxCost >= 0)
            {
                builder.Append(" AND `total_price` < " + MaxCost);
            }
            switch (FilterMode)
            {
                case 1:
                    str = "ORDER BY `asking_price` DESC";
                    break;

                default:
                    str = "ORDER BY `asking_price` ASC";
                    break;
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {

                dbClient.SetQuery("SELECT `offer_id`,`item_type`,`sprite_id`,`total_price`,`limited_number`,`limited_stack` FROM `catalog_marketplace_offers` " + builder.ToString() + " " + str + " LIMIT 500");
                dbClient.AddParameter("search_query", "%" + SearchQuery + "%");
                if (SearchQuery.Length >= 1)
                {
                    builder.Append(" AND `public_name` LIKE @search_query");
                }
                table = dbClient.GetTable();
            }

            PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItems.Clear();
            PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItemKeys.Clear();
            if (table != null)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (!PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItemKeys.Contains(Convert.ToInt32(row["offer_id"])))
                    {
                        MarketOffer item = new MarketOffer(Convert.ToInt32(row["offer_id"]), Convert.ToInt32(row["sprite_id"]), Convert.ToInt32(row["total_price"]), int.Parse(row["item_type"].ToString()), Convert.ToInt32(row["limited_number"]), Convert.ToInt32(row["limited_stack"]));
                        PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItemKeys.Add(Convert.ToInt32(row["offer_id"]));
                        PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItems.Add(item);
                    }
                }
            }

            Dictionary<int, MarketOffer> dictionary = new Dictionary<int, MarketOffer>();
            Dictionary<int, int> dictionary2 = new Dictionary<int, int>();

            foreach (MarketOffer item in PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItems)
            {
                if (dictionary.ContainsKey(item.SpriteId))
                {
                    if (dictionary[item.SpriteId].TotalPrice > item.TotalPrice)
                    {
                        dictionary.Remove(item.SpriteId);
                        dictionary.Add(item.SpriteId, item);
                    }

                    int num = dictionary2[item.SpriteId];
                    dictionary2.Remove(item.SpriteId);
                    dictionary2.Add(item.SpriteId, num + 1);
                }
                else
                {
                    dictionary.Add(item.SpriteId, item);
                    dictionary2.Add(item.SpriteId, 1);
                }
            }

            Session.SendPacket(new MarketPlaceOffersComposer(MinCost, MaxCost, dictionary, dictionary2));
        }
    }
}