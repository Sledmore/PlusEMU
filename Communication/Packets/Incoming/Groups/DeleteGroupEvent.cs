using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Groups;

using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class DeleteGroupEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Group Group = null;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(Packet.PopInt(), out Group))
            {
                Session.SendNotification("Oops, we couldn't find that group!");
                return;
            }

            if (Group.CreatorId != Session.GetHabbo().Id && !Session.GetHabbo().GetPermissions().HasRight("group_delete_override"))//Maybe a FUSE check for staff override?
            {
                Session.SendNotification("Oops, only the group owner can delete a group!");
                return;
            }

            if (Group.MemberCount >= Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("group.delete.member.limit")) && !Session.GetHabbo().GetPermissions().HasRight("group_delete_limit_override"))
            {
                Session.SendNotification("Oops, your group exceeds the maximum amount of members (" + Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("group.delete.member.limit")) + ") a group can exceed before being eligible for deletion. Seek assistance from a staff member.");
                return;
            }

            Room Room = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(Group.RoomId);

            if (Room != null)
            {
                Room.Group = null;
                Room.RoomData.Group = null;//I'm not sure if this is needed or not, becauseof inheritance, but oh well.
            }

            //Remove it from the cache.
            PlusEnvironment.GetGame().GetGroupManager().DeleteGroup(Group.Id);

            //Now the :S stuff.
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("DELETE FROM `groups` WHERE `id` = '" + Group.Id + "'");
                dbClient.RunQuery("DELETE FROM `group_memberships` WHERE `group_id` = '" + Group.Id + "'");
                dbClient.RunQuery("DELETE FROM `group_requests` WHERE `group_id` = '" + Group.Id + "'");
                dbClient.RunQuery("UPDATE `rooms` SET `group_id` = '0' WHERE `group_id` = '" + Group.Id + "' LIMIT 1");
                dbClient.RunQuery("UPDATE `user_stats` SET `groupid` = '0' WHERE `groupid` = '" + Group.Id + "' LIMIT 1");
                dbClient.RunQuery("DELETE FROM `items_groups` WHERE `group_id` = '" + Group.Id + "'");
            }

            //Unload it last.
            PlusEnvironment.GetGame().GetRoomManager().UnloadRoom(Room, true);

            //Say hey!
            Session.SendNotification("You have successfully deleted your group.");
            return;
        }
    }
}
