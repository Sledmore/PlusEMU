
using Plus.Communication.Packets.Outgoing.Navigator;

using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users;
using Plus.Communication.Packets.Incoming;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    public class RemoveFavouriteRoomEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int Id = Packet.PopInt();

            Session.GetHabbo().FavoriteRooms.Remove(Id);
            Session.SendPacket(new UpdateFavouriteRoomComposer(Id, false));

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("DELETE FROM user_favorites WHERE user_id = " + Session.GetHabbo().Id + " AND room_id = " + Id + " LIMIT 1");
            }
        }
    }
}