using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Help
{
    class GetSanctionStatusEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            //Session.SendMessage(new SanctionStatusComposer());
        }
    }
}
