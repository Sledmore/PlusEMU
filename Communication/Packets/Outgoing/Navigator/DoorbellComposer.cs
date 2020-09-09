namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class DoorbellComposer : MessageComposer
    {
        public string Username { get; }

        public DoorbellComposer(string username)
            : base(ServerPacketHeader.DoorbellMessageComposer)
        {
            this.Username = username;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(Username);
        }
    }
}
