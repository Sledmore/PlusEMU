using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class UserChangeComposer : ServerPacket
    {
        public UserChangeComposer(RoomUser User, bool Self)
            : base(ServerPacketHeader.UserChangeMessageComposer)
        {
            WriteInteger((Self) ? -1 : User.VirtualId);
           WriteString(User.GetClient().GetHabbo().Look);
           WriteString(User.GetClient().GetHabbo().Gender);
           WriteString(User.GetClient().GetHabbo().Motto);
            WriteInteger(User.GetClient().GetHabbo().GetStats().AchievementPoints);
        }
    }
}