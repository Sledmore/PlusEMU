using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Rooms.Action
{
    class KickUserEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            Room room = session.GetHabbo().CurrentRoom;
            if (room == null)
                return;

            if (!room.CheckRights(session) && room.WhoCanKick != 2 && room.Group == null)
                return;

            if (room.Group != null && !room.CheckRights(session, false, true))
                return;

            int userId = packet.PopInt();
            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(userId);
            if (user == null || user.IsBot)
                return;

            //Cannot kick owner or moderators.
            if (room.CheckRights(user.GetClient(), true) || user.GetClient().GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            room.GetRoomUserManager().RemoveUserFromRoom(user.GetClient(), true, true);
            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_SelfModKickSeen", 1);
        }
    }
}
