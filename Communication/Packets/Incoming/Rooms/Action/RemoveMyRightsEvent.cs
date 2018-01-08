using Plus.HabboHotel.Rooms;

using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;

using Plus.Communication.Packets.Outgoing.Rooms.Permissions;

namespace Plus.Communication.Packets.Incoming.Rooms.Action
{
    class RemoveMyRightsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = null;
            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            if (!Room.CheckRights(Session, false))
                return;
            
            if (Room.UsersWithRights.Contains(Session.GetHabbo().Id))
            {
                RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                if (User != null && !User.IsBot)
                {
                    User.RemoveStatus("flatctrl 1");
                    User.UpdateNeeded = true;

                    User.GetClient().SendPacket(new YouAreNotControllerComposer());
                }

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("DELETE FROM `room_rights` WHERE `user_id` = @uid AND `room_id` = @rid LIMIT 1");
                    dbClient.AddParameter("uid", Session.GetHabbo().Id);
                    dbClient.AddParameter("rid", Room.Id);
                    dbClient.RunQuery();
                }

                if (Room.UsersWithRights.Contains(Session.GetHabbo().Id))
                    Room.UsersWithRights.Remove(Session.GetHabbo().Id);
            }
        }
    }
}
