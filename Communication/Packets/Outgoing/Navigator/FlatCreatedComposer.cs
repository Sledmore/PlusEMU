namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class FlatCreatedComposer : ServerPacket
    {
        public FlatCreatedComposer(int roomId, string roomName)
            : base(ServerPacketHeader.FlatCreatedMessageComposer)
        {
            WriteInteger(roomId);
            WriteString(roomName);
        }
    }
}
