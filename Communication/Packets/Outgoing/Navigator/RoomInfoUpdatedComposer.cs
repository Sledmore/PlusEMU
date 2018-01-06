namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class RoomInfoUpdatedComposer : ServerPacket
    {
        public RoomInfoUpdatedComposer(int roomId)
            : base(ServerPacketHeader.RoomInfoUpdatedMessageComposer)
        {
            WriteInteger(roomId);
        }
    }
}
