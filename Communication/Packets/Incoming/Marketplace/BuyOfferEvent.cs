using System;
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
        public void Parse(GameClient session, ClientPacket packet)
        {
            int offerId = packet.PopInt();

            DataRow row;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `state`,`timestamp`,`total_price`,`extra_data`,`item_id`,`furni_id`,`user_id`,`limited_number`,`limited_stack` FROM `catalog_marketplace_offers` WHERE `offer_id` = @OfferId LIMIT 1");
                dbClient.AddParameter("OfferId", offerId);
                row = dbClient.GetRow();
            }

            if (row == null)
            {
                ReloadOffers(session);
                return;
            }

            if (Convert.ToString(row["state"]) == "2")
            {
                session.SendNotification("Oops, this offer is no longer available.");
                ReloadOffers(session);
                return;
            }

            if (PlusEnvironment.GetGame().GetCatalog().GetMarketplace().FormatTimestamp() > (Convert.ToDouble(row["timestamp"])))
            {
                session.SendNotification("Oops, this offer has expired..");
                ReloadOffers(session);
                return;
            }

            if (!PlusEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(row["item_id"]), out ItemData item))
            {
                session.SendNotification("Item isn't in the hotel anymore.");
                ReloadOffers(session);
                return;
            }
            else
            {
                if (Convert.ToInt32(row["user_id"]) == session.GetHabbo().Id)
                {
                    session.SendNotification("To prevent average boosting you cannot purchase your own marketplace offers.");
                    return;
                }

                if (Convert.ToInt32(row["total_price"]) > session.GetHabbo().Credits)
                {
                    session.SendNotification("Oops, you do not have enough credits for this.");
                    return;
                }

                session.GetHabbo().Credits -= Convert.ToInt32(row["total_price"]);
                session.SendPacket(new CreditBalanceComposer(session.GetHabbo().Credits));


                Item giveItem = ItemFactory.CreateSingleItem(item, session.GetHabbo(), Convert.ToString(row["extra_data"]), Convert.ToString(row["extra_data"]), Convert.ToInt32(row["furni_id"]), Convert.ToInt32(row["limited_number"]), Convert.ToInt32(row["limited_stack"]));
                if (giveItem != null)
                {
                    session.GetHabbo().GetInventoryComponent().TryAddItem(giveItem);
                    session.SendPacket(new FurniListNotificationComposer(giveItem.Id, 1));

                    session.SendPacket(new Outgoing.Catalog.PurchaseOKComposer());
                    session.SendPacket(new FurniListAddComposer(giveItem));
                    session.SendPacket(new FurniListUpdateComposer());
                }


                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `catalog_marketplace_offers` SET `state` = '2' WHERE `offer_id` = '" + offerId + "' LIMIT 1");

                    int id;
                    dbClient.SetQuery("SELECT `id` FROM `catalog_marketplace_data` WHERE `sprite` = " + item.SpriteId + " LIMIT 1;");
                    id = dbClient.GetInteger();

                    if (id > 0)
                        dbClient.RunQuery("UPDATE `catalog_marketplace_data` SET `sold` = `sold` + 1, `avgprice` = (avgprice + " + Convert.ToInt32(row["total_price"]) + ") WHERE `id` = " + id + " LIMIT 1;");
                    else
                        dbClient.RunQuery("INSERT INTO `catalog_marketplace_data` (`sprite`, `sold`, `avgprice`) VALUES ('" + item.SpriteId + "', '1', '" + Convert.ToInt32(row["total_price"]) + "')");


                    if (PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.ContainsKey(item.SpriteId) && PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.ContainsKey(item.SpriteId))
                    {
                        int num3 = PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts[item.SpriteId];
                        int num4 = (PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages[item.SpriteId] += Convert.ToInt32(row["total_price"]));

                        PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.Remove(item.SpriteId);
                        PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.Add(item.SpriteId, num4);
                        PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.Remove(item.SpriteId);
                        PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.Add(item.SpriteId, num3 + 1);
                    }
                    else
                    {
                        if (!PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.ContainsKey(item.SpriteId))
                            PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.Add(item.SpriteId, Convert.ToInt32(row["total_price"]));

                        if (!PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.ContainsKey(item.SpriteId))
                            PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.Add(item.SpriteId, 1);
                    }
                }
            }

            ReloadOffers(session);
        }


        private void ReloadOffers(GameClient session)
        {
            int minCost = -1;
            int maxCost = -1;
            string searchQuery = "";
            int filterMode = 1;


            DataTable table = null;
            StringBuilder builder = new StringBuilder();
            string str;
            builder.Append("WHERE `state` = '1' AND `timestamp` >= " + PlusEnvironment.GetGame().GetCatalog().GetMarketplace().FormatTimestampString());
            if (minCost >= 0)
            {
                builder.Append(" AND `total_price` > " + minCost);
            }
            if (maxCost >= 0)
            {
                builder.Append(" AND `total_price` < " + maxCost);
            }
            switch (filterMode)
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
                dbClient.AddParameter("search_query", "%" + searchQuery + "%");
                if (searchQuery.Length >= 1)
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

            session.SendPacket(new MarketPlaceOffersComposer(dictionary, dictionary2));
        }
    }
}