using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Groups;

using Plus.Communication.Packets.Outgoing.Groups;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Catalog;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class JoinGroupEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null)
                return;

            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(packet.PopInt(), out Group group))
                return;

            if (group.IsMember(session.GetHabbo().Id) || group.IsAdmin(session.GetHabbo().Id) || (group.HasRequest(session.GetHabbo().Id) && group.Type == GroupType.Private))
                return;

            List<Group> groups = PlusEnvironment.GetGame().GetGroupManager().GetGroupsForUser(session.GetHabbo().Id);
            if (groups.Count >= 1500)
            {
                session.SendPacket(new BroadcastMessageAlertComposer("Oops, it appears that you've hit the group membership limit! You can only join upto 1,500 groups."));
                return;
            }

            group.AddMember(session.GetHabbo().Id);

            if (group.Type == GroupType.Locked)
            {
                List<GameClient> groupAdmins = (from client in PlusEnvironment.GetGame().GetClientManager().GetClients.ToList() where client != null && client.GetHabbo() != null && @group.IsAdmin(client.GetHabbo().Id) select client).ToList();
                foreach (GameClient client in groupAdmins)
                {
                    client.SendPacket(new GroupMembershipRequestedComposer(group.Id, session.GetHabbo(), 3));
                }

                session.SendPacket(new GroupInfoComposer(group, session));
            }
            else
            {
                session.SendPacket(new GroupFurniConfigComposer(PlusEnvironment.GetGame().GetGroupManager().GetGroupsForUser(session.GetHabbo().Id)));
                session.SendPacket(new GroupInfoComposer(group, session));

                if (session.GetHabbo().CurrentRoom != null)
                    session.GetHabbo().CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(session.GetHabbo().Id));
                else
                    session.SendPacket(new RefreshFavouriteGroupComposer(session.GetHabbo().Id));
            }
        }
    }
}
