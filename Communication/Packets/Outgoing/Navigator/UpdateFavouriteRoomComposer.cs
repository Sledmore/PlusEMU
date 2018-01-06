namespace Plus.Communication.Packets.Outgoing.Navigator
{
    public class UpdateFavouriteRoomComposer : ServerPacket
    {
        public UpdateFavouriteRoomComposer(int roomId, bool added)
            : base(ServerPacketHeader.UpdateFavouriteRoomMessageComposer)
        {
            WriteInteger(roomId);
            WriteBoolean(added);
        }
    }
}