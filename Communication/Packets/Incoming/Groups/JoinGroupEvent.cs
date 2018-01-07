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
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            Group Group = null;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(Packet.PopInt(), out Group))
                return;

            if (Group.IsMember(Session.GetHabbo().Id) || Group.IsAdmin(Session.GetHabbo().Id) || (Group.HasRequest(Session.GetHabbo().Id) && Group.Type == GroupType.Private))
                return;

            List<Group> Groups = PlusEnvironment.GetGame().GetGroupManager().GetGroupsForUser(Session.GetHabbo().Id);
            if (Groups.Count >= 1500)
            {
                Session.SendPacket(new BroadcastMessageAlertComposer("Oops, it appears that you've hit the group membership limit! You can only join upto 1,500 groups."));
                return;
            }

            Group.AddMember(Session.GetHabbo().Id);

            if (Group.Type == GroupType.Locked)
            {
                List<GameClient> GroupAdmins = (from Client in PlusEnvironment.GetGame().GetClientManager().GetClients.ToList() where Client != null && Client.GetHabbo() != null && Group.IsAdmin(Client.GetHabbo().Id) select Client).ToList();
                foreach (GameClient Client in GroupAdmins)
                {
                    Client.SendPacket(new GroupMembershipRequestedComposer(Group.Id, Session.GetHabbo(), 3));
                }

                Session.SendPacket(new GroupInfoComposer(Group, Session));
            }
            else
            {
                Session.SendPacket(new GroupFurniConfigComposer(PlusEnvironment.GetGame().GetGroupManager().GetGroupsForUser(Session.GetHabbo().Id)));
                Session.SendPacket(new GroupInfoComposer(Group, Session));

                if (Session.GetHabbo().CurrentRoom != null)
                    Session.GetHabbo().CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
                else
                    Session.SendPacket(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
            }
        }
    }
}
