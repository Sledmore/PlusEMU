namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class MessengerErrorComposer : ServerPacket
    {
        public MessengerErrorComposer(int ErrorCode1, int ErrorCode2)
            : base(ServerPacketHeader.MessengerErrorMessageComposer)
        {
            WriteInteger(ErrorCode1);
            WriteInteger(ErrorCode2);
        }
    }
}
