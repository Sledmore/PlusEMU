namespace Plus.Communication.Packets.Outgoing.Navigator
{
    public class UpdateFavouriteRoomComposer : MessageComposer
    {
        public int RoomId { get; }
        public bool Added { get; }

        public UpdateFavouriteRoomComposer(int roomId, bool added)
            : base(ServerPacketHeader.UpdateFavouriteRoomMessageComposer)
        {
            this.RoomId = roomId;
            this.Added = added;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(RoomId);
            packet.WriteBoolean(Added);
        }
    }
}