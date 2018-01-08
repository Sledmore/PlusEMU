namespace Plus.Communication.Packets.Outgoing.Handshake
{
    class GenericErrorComposer : ServerPacket
    {
        public GenericErrorComposer(int errorId)
            : base(ServerPacketHeader.GenericErrorMessageComposer)
        {
            base.WriteInteger(errorId);
        }
    }
}
