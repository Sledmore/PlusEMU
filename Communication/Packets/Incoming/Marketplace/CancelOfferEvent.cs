using System;
using System.Data;
using Plus.HabboHotel.Items;
using Plus.Communication.Packets.Outgoing.Marketplace;
using Plus.Communication.Packets.Outgoing.Inventory.Furni;

using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;


namespace Plus.Communication.Packets.Incoming.Marketplace
{
    class CancelOfferEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null)
                return;

            DataRow row;
            int offerId = packet.PopInt();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `furni_id`, `item_id`, `user_id`, `extra_data`, `offer_id`, `state`, `timestamp`, `limited_number`, `limited_stack` FROM `catalog_marketplace_offers` WHERE `offer_id` = @OfferId LIMIT 1");
                dbClient.AddParameter("OfferId", offerId);
                row = dbClient.GetRow();
            }

            if (row == null)
            {
                session.SendPacket(new MarketplaceCancelOfferResultComposer(offerId, false));
                return;
            }

            if (Convert.ToInt32(row["user_id"]) != session.GetHabbo().Id)
            {
                session.SendPacket(new MarketplaceCancelOfferResultComposer(offerId, false));
                return;
            }

            if (!PlusEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(row["item_id"]), out ItemData item))
            {
                session.SendPacket(new MarketplaceCancelOfferResultComposer(offerId, false));
                return;
            }

            //PlusEnvironment.GetGame().GetCatalog().DeliverItems(Session, Item, 1, Convert.ToString(Row["extra_data"]), Convert.ToInt32(Row["limited_number"]), Convert.ToInt32(Row["limited_stack"]), Convert.ToInt32(Row["furni_id"]));

            Item giveItem = ItemFactory.CreateSingleItem(item, session.GetHabbo(), Convert.ToString(row["extra_data"]), Convert.ToString(row["extra_data"]), Convert.ToInt32(row["furni_id"]), Convert.ToInt32(row["limited_number"]), Convert.ToInt32(row["limited_stack"]));
            session.SendPacket(new FurniListNotificationComposer(giveItem.Id, 1));
            session.SendPacket(new FurniListUpdateComposer());

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM `catalog_marketplace_offers` WHERE `offer_id` = @OfferId AND `user_id` = @UserId LIMIT 1");
                dbClient.AddParameter("OfferId", offerId);
                dbClient.AddParameter("UserId", session.GetHabbo().Id);
                dbClient.RunQuery();
            }

            session.GetHabbo().GetInventoryComponent().UpdateItems(true);
            session.SendPacket(new MarketplaceCancelOfferResultComposer(offerId, true));
        }
    }
}
