using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.Avatar
{
    class DanceComposer : MessageComposer
    {
        public int VirtualId { get; }
        public int Dance { get; }

        public DanceComposer(int VirtualId, int Dance)
            : base(ServerPacketHeader.DanceMessageComposer)
        {
            this.VirtualId = VirtualId;
            this.Dance = Dance;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(VirtualId);
            packet.WriteInteger(Dance);
        }
    }
}