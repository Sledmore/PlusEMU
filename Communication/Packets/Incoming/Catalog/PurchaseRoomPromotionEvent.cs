using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Database.Interfaces;


namespace Plus.Communication.Packets.Incoming.Catalog
{
    public class PurchaseRoomPromotionEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null)
                return;

            packet.PopInt(); //pageId
            packet.PopInt(); //itemId
            int roomId = packet.PopInt();
            string name = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
            packet.PopBoolean(); //junk
            string desc = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
            int categoryId = packet.PopInt();

            if (!RoomFactory.TryGetData(roomId, out RoomData data))
                return;

            if (data.OwnerId != session.GetHabbo().Id)
                return;

            if (data.Promotion == null)
                data.Promotion = new RoomPromotion(name, desc, categoryId);
            else
            {
                data.Promotion.Name = name;
                data.Promotion.Description = desc;
                data.Promotion.TimestampExpires += 7200;
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("REPLACE INTO `room_promotions` (`room_id`,`title`,`description`,`timestamp_start`,`timestamp_expire`,`category_id`) VALUES (@room_id, @title, @description, @start, @expires, @CategoryId)");
                dbClient.AddParameter("room_id", roomId);
                dbClient.AddParameter("title", name);
                dbClient.AddParameter("description", desc);
                dbClient.AddParameter("start", data.Promotion.TimestampStarted);
                dbClient.AddParameter("expires", data.Promotion.TimestampExpires);
                dbClient.AddParameter("CategoryId", categoryId);
                dbClient.RunQuery();
            }

            if (!session.GetHabbo().GetBadgeComponent().HasBadge("RADZZ"))
                session.GetHabbo().GetBadgeComponent().GiveBadge("RADZZ", true, session);

            session.SendPacket(new PurchaseOKComposer());
            if (session.GetHabbo().InRoom && session.GetHabbo().CurrentRoomId == roomId)
                session.GetHabbo().CurrentRoom.SendPacket(new RoomEventComposer(data, data.Promotion));

            session.GetHabbo().GetMessenger().BroadcastAchievement(session.GetHabbo().Id, HabboHotel.Users.Messenger.MessengerEventTypes.EventStarted, name);
        }
    }
}