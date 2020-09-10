using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.Settings
{
    class FlatControllerRemovedComposer : MessageComposer
    {
        public int RoomId { get; }
        public int UserId { get; }
        public FlatControllerRemovedComposer(int RoomId, int UserId)
            : base(ServerPacketHeader.FlatControllerRemovedMessageComposer)
        {
            this.RoomId = RoomId;
            this.UserId = UserId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(RoomId);
            packet.WriteInteger(UserId);
        }
    }
}
