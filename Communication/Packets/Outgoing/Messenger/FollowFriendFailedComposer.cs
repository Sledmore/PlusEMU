namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class FollowFriendFailedComposer : MessageComposer
    {
        public int ErrorCode { get; }

        public FollowFriendFailedComposer(int ErrorCode)
            : base(ServerPacketHeader.FollowFriendFailedMessageComposer)
        {
            this.ErrorCode = ErrorCode;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(ErrorCode);
        }
    }
}
