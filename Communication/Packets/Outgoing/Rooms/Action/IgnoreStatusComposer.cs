namespace Plus.Communication.Packets.Outgoing.Rooms.Action
{
    class IgnoreStatusComposer : MessageComposer
    {
        public int Status { get; }
        public string Username { get; }

        public IgnoreStatusComposer(int Status, string Username)
            : base(ServerPacketHeader.IgnoreStatusMessageComposer)
        {
            this.Status = Status;
            this.Username = Username;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Status);
            packet.WriteString(Username);
        }
    }
}
