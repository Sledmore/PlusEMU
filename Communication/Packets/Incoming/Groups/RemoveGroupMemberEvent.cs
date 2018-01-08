using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Groups;
using Plus.Communication.Packets.Outgoing.Rooms.Permissions;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Cache.Type;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class RemoveGroupMemberEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int groupId = packet.PopInt();
            int userId = packet.PopInt();

            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out Group group))
                return;

            if (userId == session.GetHabbo().Id)
            {
                if (group.IsMember(userId))
                    group.DeleteMember(userId);

                if (group.IsAdmin(userId))
                {
                    if (group.IsAdmin(userId))
                        group.TakeAdmin(userId);

                    if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(group.RoomId, out Room room))
                        return;

                    RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
                    if (user != null)
                    {
                        user.RemoveStatus("flatctrl 1");
                        user.UpdateNeeded = true;

                        if (user.GetClient() != null)
                            user.GetClient().SendPacket(new YouAreControllerComposer(0));
                    }
                }

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery(
                        "DELETE FROM `group_memberships` WHERE `group_id` = @GroupId AND `user_id` = @UserId");
                    dbClient.AddParameter("GroupId", groupId);
                    dbClient.AddParameter("UserId", userId);
                    dbClient.RunQuery();
                }

                session.SendPacket(new GroupInfoComposer(group, session));
                if (session.GetHabbo().GetStats().FavouriteGroupId == groupId)
                {
                    session.GetHabbo().GetStats().FavouriteGroupId = 0;
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `user_stats` SET `groupid` = '0' WHERE `id` = @userId LIMIT 1");
                        dbClient.AddParameter("userId", userId);
                        dbClient.RunQuery();
                    }

                    if (group.AdminOnlyDeco == 0)
                    {
                        if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(group.RoomId, out Room room))
                            return;

                        RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
                        if (user != null)
                        {
                            user.RemoveStatus("flatctrl 1");
                            user.UpdateNeeded = true;

                            if (user.GetClient() != null)
                                user.GetClient().SendPacket(new YouAreControllerComposer(0));
                        }
                    }

                    if (session.GetHabbo().InRoom && session.GetHabbo().CurrentRoom != null)
                    {
                        RoomUser user = session.GetHabbo().CurrentRoom.GetRoomUserManager()
                            .GetRoomUserByHabbo(session.GetHabbo().Id);
                        if (user != null)
                            session.GetHabbo().CurrentRoom
                                .SendPacket(new UpdateFavouriteGroupComposer(group, user.VirtualId));
                        session.GetHabbo().CurrentRoom
                            .SendPacket(new RefreshFavouriteGroupComposer(session.GetHabbo().Id));
                    }
                    else
                        session.SendPacket(new RefreshFavouriteGroupComposer(session.GetHabbo().Id));
                }

                return;
            }

            if (group.CreatorId == session.GetHabbo().Id || group.IsAdmin(session.GetHabbo().Id))
            {
                if (!group.IsMember(userId))
                    return;

                if (group.IsAdmin(userId) && group.CreatorId != session.GetHabbo().Id)
                {
                    session.SendNotification(
                        "Sorry, only group creators can remove other administrators from the group.");
                    return;
                }

                if (group.IsAdmin(userId))
                    group.TakeAdmin(userId);

                if (group.IsMember(userId))
                    group.DeleteMember(userId);

                List<UserCache> members = new List<UserCache>();
                List<int> memberIds = group.GetAllMembers;
                foreach (int id in memberIds.ToList())
                {
                    UserCache groupMember = PlusEnvironment.GetGame().GetCacheManager().GenerateUser(id);
                    if (groupMember == null)
                        continue;

                    if (!members.Contains(groupMember))
                        members.Add(groupMember);
                }


                int finishIndex = 14 < members.Count ? 14 : members.Count;
                int membersCount = members.Count;

                session.SendPacket(new GroupMembersComposer(group, members.Take(finishIndex).ToList(), membersCount, 1,
                    (group.CreatorId == session.GetHabbo().Id || group.IsAdmin(session.GetHabbo().Id)), 0, ""));
            }
        }
    }
}