namespace Plus.Communication.Packets.Outgoing.Help
{
    class SubmitBullyReportComposer : ServerPacket
    {
        public SubmitBullyReportComposer(int Result)
            : base(ServerPacketHeader.SubmitBullyReportMessageComposer)
        {
            WriteInteger(Result);
        }
    }
}
