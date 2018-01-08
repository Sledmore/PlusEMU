namespace Plus.Communication.Packets.Outgoing.Misc
{
    class LatencyTestComposer : ServerPacket
    {
        public LatencyTestComposer(int testResponce)
            : base(ServerPacketHeader.LatencyResponseMessageComposer)
        {
            WriteInteger(testResponce);
        }
    }
}
