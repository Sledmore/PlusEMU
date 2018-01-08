namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class FindFriendsProcessResultComposer : ServerPacket
    {
        public FindFriendsProcessResultComposer(bool Found)
            : base(ServerPacketHeader.FindFriendsProcessResultMessageComposer)
        {
            base.WriteBoolean(Found);
        }
    }
}