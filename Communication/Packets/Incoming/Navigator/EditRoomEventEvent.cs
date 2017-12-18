using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Database.Interfaces;


namespace Plus.Communication.Packets.Incoming.Navigator
{
    class EditRoomEventEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int roomId = Packet.PopInt();
            string Name = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Packet.PopString());
            string Desc = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Packet.PopString());

            RoomData data = null;
            if (!RoomFactory.TryGetData(roomId, out data))
                return;

            if (data.OwnerId != Session.GetHabbo().Id)
                return;

            if (data.Promotion == null)
            {
                Session.SendNotification("Oops, it looks like there isn't a room promotion in this room?");
                return;
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `room_promotions` SET `title` = @title, `description` = @desc WHERE `room_id` = " + roomId + " LIMIT 1");
                dbClient.AddParameter("title", Name);
                dbClient.AddParameter("desc", Desc);
                dbClient.RunQuery();
            }

            Room Room;
            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(Convert.ToInt32(roomId), out Room))
                return;

            data.Promotion.Name = Name;
            data.Promotion.Description = Desc;
            Room.SendPacket(new RoomEventComposer(data, data.Promotion));
        }
    }
}
