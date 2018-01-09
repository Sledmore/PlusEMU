using Plus.Communication.Packets.Outgoing.Help;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Help
{
    class SendBullyReportEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.SendPacket(new SendBullyReportComposer());
        }
    }
}
