using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Handshake
{
    class PingEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.PingCount = 0;
        }
    }
}
