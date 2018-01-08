using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Groups;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Users;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class SetGroupFavouriteEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null)
                return;

            int groupId = packet.PopInt();
            if (groupId == 0)
                return;

            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out Group group))
                return;

            session.GetHabbo().GetStats().FavouriteGroupId = group.Id;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `user_stats` SET `groupid` = @groupId WHERE `id` = @userId LIMIT 1");
                dbClient.AddParameter("groupId", session.GetHabbo().GetStats().FavouriteGroupId);
                dbClient.AddParameter("userId", session.GetHabbo().Id);
                dbClient.RunQuery();
            }

            if (session.GetHabbo().InRoom && session.GetHabbo().CurrentRoom != null)
            {
                session.GetHabbo().CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(session.GetHabbo().Id));
                session.GetHabbo().CurrentRoom.SendPacket(new HabboGroupBadgesComposer(group));

                RoomUser user = session.GetHabbo().CurrentRoom.GetRoomUserManager()
                    .GetRoomUserByHabbo(session.GetHabbo().Id);
                if (user != null)
                    session.GetHabbo().CurrentRoom.SendPacket(new UpdateFavouriteGroupComposer(group, user.VirtualId));
            }
            else
                session.SendPacket(new RefreshFavouriteGroupComposer(session.GetHabbo().Id));
        }
    }
}
