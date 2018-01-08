using Plus.HabboHotel.Rooms;

using Plus.Communication.Packets.Outgoing.Rooms.Permissions;
using Plus.Communication.Packets.Outgoing.Rooms.Settings;

using Plus.Database.Interfaces;
using Plus.HabboHotel.Cache.Type;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.Action
{
    class AssignRightsEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null)
                return;

            int userId = packet.PopInt();

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(session, true))
                return;

            if (room.UsersWithRights.Contains(userId))
            {
                session.SendNotification(PlusEnvironment.GetLanguageManager().TryGetValue("room.rights.user.has_rights"));
                return;
            }

            room.UsersWithRights.Add(userId);

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("INSERT INTO `room_rights` (`room_id`,`user_id`) VALUES ('" + room.RoomId + "','" + userId + "')");
            }

            RoomUser roomUser = room.GetRoomUserManager().GetRoomUserByHabbo(userId);
            if (roomUser != null && !roomUser.IsBot)
            {
                roomUser.SetStatus("flatctrl 1");
                roomUser.UpdateNeeded = true;
                if (roomUser.GetClient() != null)
                    roomUser.GetClient().SendPacket(new YouAreControllerComposer(1));

                session.SendPacket(new FlatControllerAddedComposer(room.RoomId, roomUser.GetClient().GetHabbo().Id, roomUser.GetClient().GetHabbo().Username));
            }
            else
            {
                UserCache user =  PlusEnvironment.GetGame().GetCacheManager().GenerateUser(userId);
                if (user != null)
                    session.SendPacket(new FlatControllerAddedComposer(room.RoomId, user.Id, user.Username));
            }
        }
    }
}
