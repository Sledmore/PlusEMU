using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Groups;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class AcceptGroupMembershipEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int groupId = packet.PopInt();
            int userId = packet.PopInt();

            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out Group group))
                return;

            if (session.GetHabbo().Id != group.CreatorId && !group.IsAdmin(session.GetHabbo().Id) && !session.GetHabbo().GetPermissions().HasRight("fuse_group_accept_any"))
                return;

            if (!group.HasRequest(userId))
                return;

            Habbo habbo = PlusEnvironment.GetHabboById(userId);
            if (habbo == null)
            {
                session.SendNotification("Oops, an error occurred whilst finding this user.");
                return;
            }

            group.HandleRequest(userId, true);

            session.SendPacket(new GroupMemberUpdatedComposer(groupId, habbo, 4));
        }
    }
}