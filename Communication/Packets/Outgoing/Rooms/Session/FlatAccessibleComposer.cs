namespace Plus.Communication.Packets.Outgoing.Rooms.Session
{
    class FlatAccessibleComposer : MessageComposer
    {
        public string Username { get; }
        public FlatAccessibleComposer(string Username)
            : base(ServerPacketHeader.FlatAccessibleMessageComposer)
        {
            this.Username = Username;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(Username);
        }
    }
}
