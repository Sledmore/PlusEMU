using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Groups;
using Plus.Communication.Packets.Outgoing.Rooms.Permissions;


using Plus.HabboHotel.Users;
using Plus.HabboHotel.Cache;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class DeclineGroupMembershipEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            int UserId = Packet.PopInt();

            Group Group = null;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            if (Session.GetHabbo().Id != Group.CreatorId && !Group.IsAdmin(Session.GetHabbo().Id))
                return;

            if (!Group.HasRequest(UserId))
                return;

            Group.HandleRequest(UserId, false);
            Session.SendPacket(new UnknownGroupComposer(Group.Id, UserId));
        }
    }
}