namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class RoomInviteComposer : MessageComposer
    {
        public int SenderId { get; }
        public string Text { get; }

        public RoomInviteComposer(int SenderId, string Text)
            : base(ServerPacketHeader.RoomInviteMessageComposer)
        {
            this.SenderId = SenderId;
            this.Text = Text;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(SenderId);
            packet.WriteString(Text);
        }
    }
}
