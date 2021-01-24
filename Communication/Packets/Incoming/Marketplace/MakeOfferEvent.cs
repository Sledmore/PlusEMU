using Plus.HabboHotel.Items;
using Plus.HabboHotel.Catalog.Utilities;
using Plus.Communication.Packets.Outgoing.Marketplace;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;


namespace Plus.Communication.Packets.Incoming.Marketplace
{
    class MakeOfferEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int sellingPrice = packet.PopInt();
            packet.PopInt(); //comission
            int itemId = packet.PopInt();

            Item item = session.GetHabbo().GetInventoryComponent().GetItem(itemId);
            if (item == null)
            {
                session.SendPacket(new MarketplaceMakeOfferResultComposer(0));
                return;
            }

            if (!ItemUtility.IsRare(item))
            {
                session.SendNotification("Sorry, only Rares & LTDs can go be auctioned off in the Marketplace!");
                return;
            }

            if (sellingPrice > 70000000 || sellingPrice <= 0)
            {
                session.SendPacket(new MarketplaceMakeOfferResultComposer(0));
                return;
            }

            int comission = PlusEnvironment.GetGame().GetCatalog().GetMarketplace().CalculateComissionPrice(sellingPrice);
            int totalPrice = sellingPrice + comission;
            int itemType = 1;
            if (item.GetBaseItem().Type == 'i')
                itemType++;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `catalog_marketplace_offers` (`furni_id`,`item_id`,`user_id`,`asking_price`,`total_price`,`public_name`,`sprite_id`,`item_type`,`timestamp`,`extra_data`,`limited_number`,`limited_stack`) VALUES ('" + itemId + "','" + item.BaseItem + "','" + session.GetHabbo().Id + "','" + sellingPrice + "','" + totalPrice + "',@public_name,'" + item.GetBaseItem().SpriteId + "','" + itemType + "','" + PlusEnvironment.GetUnixTimestamp() + "',@extra_data, '" + item.LimitedNo + "', '" + item.LimitedTot + "')");
                dbClient.AddParameter("public_name", item.GetBaseItem().PublicName);
                dbClient.AddParameter("extra_data", item.ExtraData);
                dbClient.RunQuery();

                dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + itemId + "' AND `user_id` = '" + session.GetHabbo().Id + "' LIMIT 1");
            }

            session.GetHabbo().GetInventoryComponent().RemoveItem(itemId);
            session.SendPacket(new MarketplaceMakeOfferResultComposer(1));
        }
    }
}