namespace Plus.Communication.Packets.Outgoing.Navigator
{
    public class UpdateFavouriteRoomComposer : ServerPacket
    {
        public UpdateFavouriteRoomComposer(int RoomId, bool Added)
            : base(ServerPacketHeader.UpdateFavouriteRoomMessageComposer)
        {
            base.WriteInteger(RoomId);
            base.WriteBoolean(Added);
        }
    }
}