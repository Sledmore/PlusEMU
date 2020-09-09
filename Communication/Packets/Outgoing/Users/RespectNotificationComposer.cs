namespace Plus.Communication.Packets.Outgoing.Users
{
    class RespectNotificationComposer : MessageComposer
    {
        public int UserId { get; }
        public int Respect { get; }
        public RespectNotificationComposer(int userId, int respect)
            : base(ServerPacketHeader.RespectNotificationMessageComposer)
        {
            this.UserId = userId;
            this.Respect = respect;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(UserId);
            packet.WriteInteger(Respect);
        }
    }
}
