namespace Plus.Communication.Packets.Outgoing.Help
{
    class SubmitBullyReportComposer : MessageComposer
    {
        public int Result { get; }

        public SubmitBullyReportComposer(int Result)
            : base(ServerPacketHeader.SubmitBullyReportMessageComposer)
        {
            this.Result = Result;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Result);
        }
    }
}
