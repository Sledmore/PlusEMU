namespace Plus.Communication.Packets.Outgoing.Help
{
    class SendBullyReportComposer : MessageComposer
    {
        public SendBullyReportComposer()
            : base(ServerPacketHeader.SendBullyReportMessageComposer)
        {
            
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(0);//0-3, sends 0 on Habbo for this purpose.
        }
    }
}
