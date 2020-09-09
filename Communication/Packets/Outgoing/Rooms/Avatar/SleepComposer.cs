using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.Avatar
{
    public class SleepComposer : MessageComposer
    {
        public int VirtualId { get; }
        public bool IsSleeping { get; }

        public SleepComposer(int VirtualId, bool IsSleeping)
            : base(ServerPacketHeader.SleepMessageComposer)
        {
            this.VirtualId = VirtualId;
            this.IsSleeping = IsSleeping;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(VirtualId);
            packet.WriteBoolean(IsSleeping);
        }
    }
}