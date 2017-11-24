using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Groups;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class GetGroupInfoEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            bool NewWindow = Packet.PopBoolean();

            Group Group = null;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            Session.SendPacket(new GroupInfoComposer(Group, Session, NewWindow));     
        }
    }
}
