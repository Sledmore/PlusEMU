namespace Plus.Communication.Packets.Outgoing.Users
{
    class RespectNotificationComposer : ServerPacket
    {
        public RespectNotificationComposer(int userId, int respect)
            : base(ServerPacketHeader.RespectNotificationMessageComposer)
        {
            WriteInteger(userId);
            WriteInteger(respect);
        }
    }
}
