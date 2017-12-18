using System;
using System.Data;

using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

using Plus.Database.Interfaces;


namespace Plus.Communication.Packets.Incoming.Catalog
{
    public class PurchaseRoomPromotionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            int PageId = Packet.PopInt();
            int ItemId = Packet.PopInt();
            int RoomId = Packet.PopInt();
            string Name = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Packet.PopString());
            bool junk3 = Packet.PopBoolean();
            string Desc = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Packet.PopString());
            int CategoryId = Packet.PopInt();

            RoomData Data = null;
            if (!RoomFactory.TryGetData(RoomId, out Data))
                return;

            if (Data.OwnerId != Session.GetHabbo().Id)
                return;

            if (Data.Promotion == null)
                Data.Promotion = new RoomPromotion(Name, Desc, CategoryId);
            else
            {
                Data.Promotion.Name = Name;
                Data.Promotion.Description = Desc;
                Data.Promotion.TimestampExpires += 7200;
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("REPLACE INTO `room_promotions` (`room_id`,`title`,`description`,`timestamp_start`,`timestamp_expire`,`category_id`) VALUES (@room_id, @title, @description, @start, @expires, @CategoryId)");
                dbClient.AddParameter("room_id", RoomId);
                dbClient.AddParameter("title", Name);
                dbClient.AddParameter("description", Desc);
                dbClient.AddParameter("start", Data.Promotion.TimestampStarted);
                dbClient.AddParameter("expires", Data.Promotion.TimestampExpires);
                dbClient.AddParameter("CategoryId", CategoryId);
                dbClient.RunQuery();
            }

            if (!Session.GetHabbo().GetBadgeComponent().HasBadge("RADZZ"))
                Session.GetHabbo().GetBadgeComponent().GiveBadge("RADZZ", true, Session);

            Session.SendPacket(new PurchaseOKComposer());
            if (Session.GetHabbo().InRoom && Session.GetHabbo().CurrentRoomId == RoomId)
                Session.GetHabbo().CurrentRoom.SendPacket(new RoomEventComposer(Data, Data.Promotion));

            Session.GetHabbo().GetMessenger().BroadcastAchievement(Session.GetHabbo().Id, HabboHotel.Users.Messenger.MessengerEventTypes.EventStarted, Name);
        }
    }
}