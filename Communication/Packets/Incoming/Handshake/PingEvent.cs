namespace Plus.Communication.Packets.Incoming.Handshake
{
    class PingEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.PingCount = 0;
        }
    }
}
