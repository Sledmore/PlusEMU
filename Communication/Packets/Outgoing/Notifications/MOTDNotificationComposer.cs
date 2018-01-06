namespace Plus.Communication.Packets.Outgoing.Notifications
{
    class MotdNotificationComposer : ServerPacket
    {
        public MotdNotificationComposer(string message)
            : base(ServerPacketHeader.MOTDNotificationMessageComposer)
        {
            WriteInteger(1);
            WriteString(message);
        }
    }
}
