using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Rooms.Settings;
using Plus.Communication.Packets.Outgoing.Rooms.Permissions;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;


namespace Plus.Communication.Packets.Incoming.Rooms.Action
{
    class RemoveRightsEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(session, true))
                return;

            int amount = packet.PopInt();
            for (int i = 0; (i < amount && i <= 100); i++)
            {
                int userId = packet.PopInt();
                if (userId > 0 && room.UsersWithRights.Contains(userId))
                {
                    RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(userId);
                    if (user != null && !user.IsBot)
                    {
                        user.RemoveStatus("flatctrl 1");
                        user.UpdateNeeded = true;


                        user.GetClient().SendPacket(new YouAreControllerComposer(0));
                    }

                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("DELETE FROM `room_rights` WHERE `user_id` = @uid AND `room_id` = @rid LIMIT 1");
                        dbClient.AddParameter("uid", userId);
                        dbClient.AddParameter("rid", room.Id);
                        dbClient.RunQuery();
                    }

                    if (room.UsersWithRights.Contains(userId))
                        room.UsersWithRights.Remove(userId);

                    session.SendPacket(new FlatControllerRemovedComposer(room.Id, userId));
                }
            }
        }
    }
}
