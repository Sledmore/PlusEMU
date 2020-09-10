namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class MessengerErrorComposer : MessageComposer
    {
        public int ErrorCode1 { get; }
        public int ErrorCode2 { get; }

        public MessengerErrorComposer(int ErrorCode1, int ErrorCode2)
            : base(ServerPacketHeader.MessengerErrorMessageComposer)
        {
            this.ErrorCode1 = ErrorCode1;
            this.ErrorCode2 = ErrorCode2;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(ErrorCode1);
            packet.WriteInteger(ErrorCode2);
        }
    }
}
