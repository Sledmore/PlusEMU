namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class CanCreateRoomComposer : ServerPacket
    {
        public CanCreateRoomComposer(bool error, int maxRoomsPerUser)
            : base(ServerPacketHeader.CanCreateRoomMessageComposer)
        {
            WriteInteger(error ? 1 : 0);
            WriteInteger(maxRoomsPerUser);
        }
    }
}
