using Plus.Communication.Packets.Outgoing.Help;

namespace Plus.Communication.Packets.Incoming.Help
{
    class SendBullyReportEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new SendBullyReportComposer());
        }
    }
}
