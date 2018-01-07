using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Groups;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class GetGroupInfoEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int groupId = packet.PopInt();
            bool newWindow = packet.PopBoolean();

            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out Group group))
                return;

            session.SendPacket(new GroupInfoComposer(group, session, newWindow));     
        }
    }
}
