using Plus.Communication.Packets.Outgoing.Navigator;

using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    public class RemoveFavouriteRoomEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int id = packet.PopInt();

            session.GetHabbo().FavoriteRooms.Remove(id);
            session.SendPacket(new UpdateFavouriteRoomComposer(id, false));

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("DELETE FROM user_favorites WHERE user_id = " + session.GetHabbo().Id + " AND room_id = " + id + " LIMIT 1");
            }
        }
    }
}