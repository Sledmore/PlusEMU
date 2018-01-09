using System;
using System.Text;
using System.Data;
using System.Collections.Generic;

using Plus.HabboHotel.Catalog.Marketplace;
using Plus.Communication.Packets.Outgoing.Marketplace;

using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;


namespace Plus.Communication.Packets.Incoming.Marketplace
{
    class GetOffersEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int minCost = packet.PopInt();
            int maxCost = packet.PopInt();
            string searchQuery = packet.PopString();
            int filterMode = packet.PopInt();


            DataTable table;
            StringBuilder builder = new StringBuilder();
            string str;
            builder.Append("WHERE `state` = '1' AND `timestamp` >= " + PlusEnvironment.GetGame().GetCatalog().GetMarketplace().FormatTimestampString());
            if (minCost >= 0)
                builder.Append(" AND `total_price` > " + minCost);


            if (maxCost >= 0)
                builder.Append(" AND `total_price` < " + maxCost);

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

                dbClient.SetQuery("SELECT `offer_id`, `item_type`, `sprite_id`, `total_price`, `limited_number`,`limited_stack` FROM `catalog_marketplace_offers` " + builder + " " + str + " LIMIT 500");
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
                        PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItemKeys.Add(Convert.ToInt32(row["offer_id"]));
                        PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItems.Add(new MarketOffer(Convert.ToInt32(row["offer_id"]), Convert.ToInt32(row["sprite_id"]), Convert.ToInt32(row["total_price"]), int.Parse(row["item_type"].ToString()), Convert.ToInt32(row["limited_number"]), Convert.ToInt32(row["limited_stack"])));
                    }
                }
            }

            Dictionary<int, MarketOffer> dictionary = new Dictionary<int, MarketOffer>();
            Dictionary<int, int> dictionary2 = new Dictionary<int, int>();

            foreach (MarketOffer item in PlusEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItems)
            {
                if (dictionary.ContainsKey(item.SpriteId))
                {
                    if (item.LimitedNumber > 0)
                    {
                        if (!dictionary.ContainsKey(item.OfferID))
                            dictionary.Add(item.OfferID, item);
                        if (!dictionary2.ContainsKey(item.OfferID))
                            dictionary2.Add(item.OfferID, 1);
                    }
                    else
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
                }
                else
                {
                    if (!dictionary.ContainsKey(item.SpriteId))
                        dictionary.Add(item.SpriteId, item);
                    if (!dictionary2.ContainsKey(item.SpriteId))
                        dictionary2.Add(item.SpriteId, 1);
                }
            }

            session.SendPacket(new MarketPlaceOffersComposer(dictionary, dictionary2));
        }
    }
}
