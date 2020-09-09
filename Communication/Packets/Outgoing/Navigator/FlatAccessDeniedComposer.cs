namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class FlatAccessDeniedComposer : MessageComposer
    {
        public string Username { get; }

        public FlatAccessDeniedComposer(string username)
            : base(ServerPacketHeader.FlatAccessDeniedMessageComposer)
        {
            this.Username = username;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(Username);
        }
    }
}
