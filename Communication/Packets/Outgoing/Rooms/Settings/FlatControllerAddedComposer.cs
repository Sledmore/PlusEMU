namespace Plus.Communication.Packets.Outgoing.Rooms.Settings
{
    class FlatControllerAddedComposer : ServerPacket
    {
        public FlatControllerAddedComposer(int RoomId, int UserId, string Username)
            : base(ServerPacketHeader.FlatControllerAddedMessageComposer)
        {
            WriteInteger(RoomId);
            WriteInteger(UserId);
           WriteString(Username);
        }
    }
}
