using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Items;
using Plus.HabboHotel.Catalog.Utilities;
using Plus.Communication.Packets.Outgoing.Marketplace;
using Plus.Database.Interfaces;


namespace Plus.Communication.Packets.Incoming.Marketplace
{
    class MakeOfferEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int SellingPrice = Packet.PopInt();
            int ComissionPrice = Packet.PopInt();
            int ItemId = Packet.PopInt();

            Item Item = Session.GetHabbo().GetInventoryComponent().GetItem(ItemId);
            if (Item == null)
            {
                Session.SendPacket(new MarketplaceMakeOfferResultComposer(0));
                return;
            }

            if (!ItemUtility.IsRare(Item))
            {
                Session.SendNotification("Sorry, only Rares & LTDs can go be auctioned off in the Marketplace!");
                return;
            }

            if (SellingPrice > 70000000 || SellingPrice == 0)
            {
                Session.SendPacket(new MarketplaceMakeOfferResultComposer(0));
                return;
            }

            int Comission = PlusEnvironment.GetGame().GetCatalog().GetMarketplace().CalculateComissionPrice((float)SellingPrice);
            int TotalPrice = SellingPrice + Comission;
            int ItemType = 1;
            if (Item.GetBaseItem().Type == 'i')
                ItemType++;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `catalog_marketplace_offers` (`furni_id`,`item_id`,`user_id`,`asking_price`,`total_price`,`public_name`,`sprite_id`,`item_type`,`timestamp`,`extra_data`,`limited_number`,`limited_stack`) VALUES ('" + ItemId + "','" + Item.BaseItem + "','" + Session.GetHabbo().Id + "','" + SellingPrice + "','" + TotalPrice + "',@public_name,'" + Item.GetBaseItem().SpriteId + "','" + ItemType + "','" + PlusEnvironment.GetUnixTimestamp() + "',@extra_data, '" + Item.LimitedNo + "', '" + Item.LimitedTot + "')");
                dbClient.AddParameter("public_name", Item.GetBaseItem().PublicName);
                dbClient.AddParameter("extra_data", Item.ExtraData);
                dbClient.RunQuery();

                dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + ItemId + "' AND `user_id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }

            Session.GetHabbo().GetInventoryComponent().RemoveItem(ItemId);
            Session.SendPacket(new MarketplaceMakeOfferResultComposer(1));
        }
    }
}