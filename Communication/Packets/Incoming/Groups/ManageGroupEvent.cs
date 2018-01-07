using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Groups;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class ManageGroupEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int groupId = packet.PopInt();

            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out Group group))
                return;

            if (group.CreatorId != session.GetHabbo().Id && !session.GetHabbo().GetPermissions().HasRight("group_management_override"))
                return;

            session.SendPacket(new ManageGroupComposer(group, group.Badge.Replace("b", "").Split('s')));
        }
    }
}
