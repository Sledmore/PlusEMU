using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Help
{
    class GetSanctionStatusEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            //Session.SendMessage(new SanctionStatusComposer());
        }
    }
}
