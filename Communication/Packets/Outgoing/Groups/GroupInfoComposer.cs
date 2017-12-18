using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Outgoing.Groups
{
    class GroupInfoComposer : ServerPacket
    {
        public GroupInfoComposer(Group Group, GameClient Session, bool NewWindow = false)
            : base(ServerPacketHeader.GroupInfoMessageComposer)
        {
            DateTime Origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Group.CreateTime);

            base.WriteInteger(Group.Id);
            base.WriteBoolean(true);
            base.WriteInteger(Group.GroupType == GroupType.Open ? 0 : Group.GroupType == GroupType.Locked ? 1 : 2);
            base.WriteString(Group.Name);
            base.WriteString(Group.Description);
            base.WriteString(Group.Badge);
            base.WriteInteger(Group.RoomId);
            base.WriteString(Group.GetRoom() != null ? Group.GetRoom().Name : "No room found..");    // room name
            base.WriteInteger(Group.CreatorId == Session.GetHabbo().Id ? 3 : Group.HasRequest(Session.GetHabbo().Id) ? 2 : Group.IsMember(Session.GetHabbo().Id) ? 1 : 0);
            base.WriteInteger(Group.MemberCount); // Members
            base.WriteBoolean(false);//?? CHANGED
            base.WriteString(Origin.Day + "-" + Origin.Month + "-" + Origin.Year);
            base.WriteBoolean(Group.CreatorId == Session.GetHabbo().Id);
            base.WriteBoolean(Group.IsAdmin(Session.GetHabbo().Id)); // admin
            base.WriteString(PlusEnvironment.GetUsernameById(Group.CreatorId));
            base.WriteBoolean(NewWindow); // Show group info
            base.WriteBoolean(Group.AdminOnlyDeco == 0); // Any user can place furni in home room
            base.WriteInteger(Group.CreatorId == Session.GetHabbo().Id ? Group.RequestCount : Group.IsAdmin(Session.GetHabbo().Id) ? Group.RequestCount : Group.IsMember(Session.GetHabbo().Id) ? 0 : 0); // Pending users
            //base.WriteInteger(0);//what the fuck
            base.WriteBoolean(Group != null ? Group.ForumEnabled : true);//HabboTalk.
        }
    }
}