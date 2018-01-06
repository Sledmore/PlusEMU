using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Groups;
using Plus.Communication.Packets.Outgoing.Rooms.Permissions;

using Plus.Database.Interfaces;


namespace Plus.Communication.Packets.Incoming.Groups
{
    class UpdateGroupSettingsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();

            Group Group = null;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            if (Group.CreatorId != Session.GetHabbo().Id)
                return;

            int Type = Packet.PopInt();
            int FurniOptions = Packet.PopInt();

            switch (Type)
            {
                default:
                case 0:
                    Group.Type = GroupType.Open;
                    break;
                case 1:
                    Group.Type = GroupType.Locked;
                    break;
                case 2:
                    Group.Type = GroupType.Private;
                    break;
            }

            if (Group.Type != GroupType.Locked)
            {
                if (Group.GetRequests.Count > 0)
                {
                    foreach (int UserId in Group.GetRequests.ToList())
                    {
                        Group.HandleRequest(UserId, false);
                    }

                    Group.ClearRequests();
                }
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `groups` SET `state` = @GroupState, `admindeco` = @AdminDeco WHERE `id` = @groupId LIMIT 1");
                dbClient.AddParameter("GroupState", (Group.Type == GroupType.Open ? 0 : Group.Type == GroupType.Locked ? 1 : 2).ToString());
                dbClient.AddParameter("AdminDeco", (FurniOptions == 1 ? 1 : 0).ToString());
                dbClient.AddParameter("groupId", Group.Id);
                dbClient.RunQuery();
            }

            Group.AdminOnlyDeco = FurniOptions;

            Room Room;
            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out Room))
                return;

            foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers().ToList())
            {
                if (Room.OwnerId == User.UserId || Group.IsAdmin(User.UserId) || !Group.IsMember(User.UserId))
                    continue;

                if (FurniOptions == 1)
                {
                    User.RemoveStatus("flatctrl 1");
                    User.UpdateNeeded = true;

                    User.GetClient().SendPacket(new YouAreControllerComposer(0));
                }
                else if (FurniOptions == 0 && !User.Statusses.ContainsKey("flatctrl 1"))
                {
                    User.SetStatus("flatctrl 1", "");
                    User.UpdateNeeded = true;

                    User.GetClient().SendPacket(new YouAreControllerComposer(1));
                }
            }

            Session.SendPacket(new GroupInfoComposer(Group, Session));
        }
    }
}