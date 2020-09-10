namespace Plus.Communication.Packets.Outgoing.Misc
{
    class LatencyTestComposer : MessageComposer
    {
        public int TestResponse { get; }

        public LatencyTestComposer(int testResponce)
            : base(ServerPacketHeader.LatencyResponseMessageComposer)
        {
            this.TestResponse = testResponce;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(TestResponse);
        }
    }
}
