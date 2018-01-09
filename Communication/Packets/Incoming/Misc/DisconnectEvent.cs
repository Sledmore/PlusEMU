using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Misc
{
    class DisconnectEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.Disconnect();
        }
    }
}
