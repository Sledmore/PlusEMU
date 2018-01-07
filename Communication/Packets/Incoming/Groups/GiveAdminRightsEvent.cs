using Plus.HabboHotel.Users;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Groups;
using Plus.Communication.Packets.Outgoing.Rooms.Permissions;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class GiveAdminRightsEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int groupId = packet.PopInt();
            int userId = packet.PopInt();

            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out Group group))
                return;

            if (session.GetHabbo().Id != group.CreatorId || !group.IsMember(userId))
                return;

            Habbo habbo = PlusEnvironment.GetHabboById(userId);
            if (habbo == null)
            {
                session.SendNotification("Oops, an error occurred whilst finding this user.");
                return;
            }

            group.MakeAdmin(userId);

            if (PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(group.RoomId, out Room room))
            {
                RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(userId);
                if (user != null)
                {
                    if (!user.Statusses.ContainsKey("flatctrl 3"))
                        user.SetStatus("flatctrl 3");
                    user.UpdateNeeded = true;
                    if (user.GetClient() != null)
                        user.GetClient().SendPacket(new YouAreControllerComposer(3));
                }
            }

            session.SendPacket(new GroupMemberUpdatedComposer(groupId, habbo, 1));
        }
    }
}
