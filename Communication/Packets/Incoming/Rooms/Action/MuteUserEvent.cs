using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Rooms.Action
{
    class MuteUserEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            int userId = packet.PopInt();
            packet.PopInt(); //roomId
            int time = packet.PopInt();

            Room room = session.GetHabbo().CurrentRoom;
            if (room == null)
                return;

            if (((room.WhoCanMute == 0 && !room.CheckRights(session, true) && room.Group == null) || (room.WhoCanMute == 1 && !room.CheckRights(session)) && room.Group == null) || (room.Group != null && !room.CheckRights(session, false, true)))
                return;

            RoomUser target = room.GetRoomUserManager().GetRoomUserByHabbo(PlusEnvironment.GetUsernameById(userId));
            if (target == null)
                return;
            else if (target.GetClient().GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            if (room.MutedUsers.ContainsKey(userId))
            {
                if (room.MutedUsers[userId] < PlusEnvironment.GetUnixTimestamp())
                    room.MutedUsers.Remove(userId);
                else
                    return;
            }

            room.MutedUsers.Add(userId, (PlusEnvironment.GetUnixTimestamp() + (time * 60)));
          
            target.GetClient().SendWhisper("The room owner has muted you for " + time + " minutes!");
            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_SelfModMuteSeen", 1);
        }
    }
}
