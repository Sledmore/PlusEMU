using Plus.HabboHotel.Rooms;

using Plus.Communication.Packets.Outgoing.Rooms.Permissions;
using Plus.Communication.Packets.Outgoing.Rooms.Settings;

using Plus.Database.Interfaces;
using Plus.HabboHotel.Cache.Type;

namespace Plus.Communication.Packets.Incoming.Rooms.Action
{
    class AssignRightsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
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

            RoomUser RoomUser = room.GetRoomUserManager().GetRoomUserByHabbo(userId);
            if (RoomUser != null && !RoomUser.IsBot)
            {
                RoomUser.SetStatus("flatctrl 1", "");
                RoomUser.UpdateNeeded = true;
                if (RoomUser.GetClient() != null)
                    RoomUser.GetClient().SendPacket(new YouAreControllerComposer(1));

                session.SendPacket(new FlatControllerAddedComposer(room.RoomId, RoomUser.GetClient().GetHabbo().Id, RoomUser.GetClient().GetHabbo().Username));
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
