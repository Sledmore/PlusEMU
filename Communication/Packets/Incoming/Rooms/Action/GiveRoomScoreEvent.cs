using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Navigator;

using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;


namespace Plus.Communication.Packets.Incoming.Rooms.Action
{
    class GiveRoomScoreEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;

            if (session.GetHabbo().RatedRooms.Contains(room.RoomId) || room.CheckRights(session, true))
                return;

            int rating = packet.PopInt();
            switch (rating)
            {
                case -1:
                    room.Score--;
                    break;
                case 1:
                    room.Score++;
                    break;
                default:
                    return;
            }
          
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE rooms SET score = '" + room.Score + "' WHERE id = '" + room.RoomId + "' LIMIT 1");
            }

            session.GetHabbo().RatedRooms.Add(room.RoomId);        
            session.SendPacket(new RoomRatingComposer(room.Score, !(session.GetHabbo().RatedRooms.Contains(room.RoomId) || room.CheckRights(session, true))));
        }
    }
}
