using System;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;


namespace Plus.Communication.Packets.Incoming.Navigator
{
    class EditRoomEventEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int roomId = packet.PopInt();
            string name = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
            string desc = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());

            if (!RoomFactory.TryGetData(roomId, out RoomData data))
                return;

            if (data.OwnerId != session.GetHabbo().Id)
                return;

            if (data.Promotion == null)
            {
                session.SendNotification("Oops, it looks like there isn't a room promotion in this room?");
                return;
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `room_promotions` SET `title` = @title, `description` = @desc WHERE `room_id` = " + roomId + " LIMIT 1");
                dbClient.AddParameter("title", name);
                dbClient.AddParameter("desc", desc);
                dbClient.RunQuery();
            }

            Room room;
            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(Convert.ToInt32(roomId), out room))
                return;

            data.Promotion.Name = name;
            data.Promotion.Description = desc;
            room.SendPacket(new RoomEventComposer(data, data.Promotion));
        }
    }
}
