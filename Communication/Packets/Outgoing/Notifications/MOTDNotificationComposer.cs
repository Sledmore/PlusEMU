namespace Plus.Communication.Packets.Outgoing.Notifications
{
    class MotdNotificationComposer : MessageComposer
    {
        public string Message { get; }
        public MotdNotificationComposer(string message)
            : base(ServerPacketHeader.MOTDNotificationMessageComposer)
        {
            this.Message = message;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(1);
            packet.WriteString(Message);
        }
    }
}
