namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class FindFriendsProcessResultComposer : MessageComposer
    {
        public bool Found { get; }

        public FindFriendsProcessResultComposer(bool Found)
            : base(ServerPacketHeader.FindFriendsProcessResultMessageComposer)
        {
            this.Found = Found;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteBoolean(Found);
        }
    }
}