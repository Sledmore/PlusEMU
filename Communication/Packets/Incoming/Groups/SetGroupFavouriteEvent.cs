using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Groups;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Users;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class SetGroupFavouriteEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null)
                return;

            int GroupId = Packet.PopInt();
            if (GroupId == 0)
                return;

            Group Group = null;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            Session.GetHabbo().GetStats().FavouriteGroupId = Group.Id;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `user_stats` SET `groupid` = @groupId WHERE `id` = @userId LIMIT 1");
                dbClient.AddParameter("groupId", Session.GetHabbo().GetStats().FavouriteGroupId);
                dbClient.AddParameter("userId", Session.GetHabbo().Id);
                dbClient.RunQuery();
            }

            if (Session.GetHabbo().InRoom && Session.GetHabbo().CurrentRoom != null)
            {
                Session.GetHabbo().CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
                if (Group != null)
                {
                    Session.GetHabbo().CurrentRoom.SendPacket(new HabboGroupBadgesComposer(Group));

                    RoomUser User = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                    if (User != null)
                    Session.GetHabbo().CurrentRoom.SendPacket(new UpdateFavouriteGroupComposer(Group, User.VirtualId));
                }
            }
            else
                Session.SendPacket(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
        }
    }
}
