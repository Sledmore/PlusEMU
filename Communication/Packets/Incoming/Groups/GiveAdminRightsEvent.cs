using Plus.HabboHotel.Users;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Groups;
using Plus.Communication.Packets.Outgoing.Rooms.Permissions;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class GiveAdminRightsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            int groupId = packet.PopInt();
            int userId = packet.PopInt();

            Group group = null;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out group))
                return;

            if (session.GetHabbo().Id != group.CreatorId || !group.IsMember(userId))
                return;

            Habbo Habbo = PlusEnvironment.GetHabboById(userId);
            if (Habbo == null)
            {
                session.SendNotification("Oops, an error occurred whilst finding this user.");
                return;
            }

            group.MakeAdmin(userId);
          
            Room room = null;
            if (PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(group.RoomId, out room))
            {
                RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(userId);
                if (user != null)
                {
                    if (!user.Statusses.ContainsKey("flatctrl 3"))
                        user.SetStatus("flatctrl 3", "");
                    user.UpdateNeeded = true;
                    if (user.GetClient() != null)
                        user.GetClient().SendPacket(new YouAreControllerComposer(3));
                }
            }

            session.SendPacket(new GroupMemberUpdatedComposer(groupId, Habbo, 1));
        }
    }
}
