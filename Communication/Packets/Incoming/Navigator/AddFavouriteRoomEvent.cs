
using Plus.Communication.Packets.Outgoing.Navigator;

using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.Communication.Packets.Incoming;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    public class AddFavouriteRoomEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null)
                return;

            int RoomId = Packet.PopInt();

            RoomData Data = PlusEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);

            if (Data == null || Session.GetHabbo().FavoriteRooms.Count >= 30 || Session.GetHabbo().FavoriteRooms.Contains(RoomId))
            {
                // send packet that favourites is full.
                return;
            }

            Session.GetHabbo().FavoriteRooms.Add(RoomId);
            Session.SendPacket(new UpdateFavouriteRoomComposer(RoomId, true));

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("INSERT INTO user_favorites (user_id,room_id) VALUES (" + Session.GetHabbo().Id + "," + RoomId + ")");
            }
        }
    }
}