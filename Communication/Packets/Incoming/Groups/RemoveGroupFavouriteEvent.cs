using Plus.Communication.Packets.Outgoing.Groups;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class RemoveGroupFavouriteEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.GetHabbo().GetStats().FavouriteGroupId = 0;

            if (session.GetHabbo().InRoom)
            {
                RoomUser user = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
                if (user != null)
                    session.GetHabbo().CurrentRoom.SendPacket(new UpdateFavouriteGroupComposer(null, user.VirtualId));
                session.GetHabbo().CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(session.GetHabbo().Id));
            }
            else
                session.SendPacket(new RefreshFavouriteGroupComposer(session.GetHabbo().Id));
        }
    }
}
