namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class FindFriendsProcessResultComposer : ServerPacket
    {
        public FindFriendsProcessResultComposer(bool Found)
            : base(ServerPacketHeader.FindFriendsProcessResultMessageComposer)
        {
            WriteBoolean(Found);
        }
    }
}