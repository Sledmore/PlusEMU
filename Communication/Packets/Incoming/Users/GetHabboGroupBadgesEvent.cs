using System.Collections.Generic;
using System.Linq;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Users;

namespace Plus.Communication.Packets.Incoming.Users
{
    class GetHabboGroupBadgesEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || !session.GetHabbo().InRoom)
                return;

            Room room = session.GetHabbo().CurrentRoom;
            if (room == null)
                return;

            Dictionary<int, string> badges = new Dictionary<int, string>();
            foreach (RoomUser user in room.GetRoomUserManager().GetRoomUsers().ToList())
            {
                if (user.IsBot || user.IsPet || user.GetClient() == null || user.GetClient().GetHabbo() == null)
                    continue;

                if (user.GetClient().GetHabbo().GetStats().FavouriteGroupId == 0 || badges.ContainsKey(user.GetClient().GetHabbo().GetStats().FavouriteGroupId))
                    continue;

                if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(user.GetClient().GetHabbo().GetStats().FavouriteGroupId, out Group group))
                    continue;

                if (!badges.ContainsKey(group.Id))
                    badges.Add(group.Id, group.Badge);
            }

            if (session.GetHabbo().GetStats().FavouriteGroupId > 0)
            {
                if (PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(session.GetHabbo().GetStats().FavouriteGroupId, out Group group))
                {
                    if (!badges.ContainsKey(group.Id))
                        badges.Add(group.Id, group.Badge);
                }
            }

            room.SendPacket(new HabboGroupBadgesComposer(badges));
            session.SendPacket(new HabboGroupBadgesComposer(badges));
        }
    }
}