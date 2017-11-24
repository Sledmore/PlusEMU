using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.Avatar
{
    public class SleepComposer : ServerPacket
    {
        public SleepComposer(RoomUser User, bool IsSleeping)
            : base(ServerPacketHeader.SleepMessageComposer)
        {
            base.WriteInteger(User.VirtualId);
            base.WriteBoolean(IsSleeping);
        }
    }
}