using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Settings;
using Plus.Communication.Packets.Outgoing.Rooms.Permissions;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;


namespace Plus.Communication.Packets.Incoming.Rooms.Action
{
    class RemoveAllRightsEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room instance))
                return;

            if (!instance.CheckRights(session, true))
                return;

            foreach (int userId in new List<int>(instance.UsersWithRights))
            {
                RoomUser user = instance.GetRoomUserManager().GetRoomUserByHabbo(userId);
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
                    dbClient.AddParameter("rid", instance.Id);
                    dbClient.RunQuery();
                }

                session.SendPacket(new FlatControllerRemovedComposer(instance.Id, userId));
                session.SendPacket(new RoomRightsListComposer(instance));
                session.SendPacket(new UserUpdateComposer(instance.GetRoomUserManager().GetUserList().ToList()));
            }

            if (instance.UsersWithRights.Count > 0)
                instance.UsersWithRights.Clear();
        }
    }
}
