namespace Plus.Communication.Packets.Outgoing.Rooms.Settings
{
    class UnbanUserFromRoomComposer : ServerPacket
    {
        public UnbanUserFromRoomComposer(int RoomId, int UserId)
            : base(ServerPacketHeader.UnbanUserFromRoomMessageComposer)
        {
            base.WriteInteger(RoomId);
            base.WriteInteger(UserId);
        }
    }
}
