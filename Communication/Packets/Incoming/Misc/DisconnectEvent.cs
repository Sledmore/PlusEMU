namespace Plus.Communication.Packets.Incoming.Misc
{
    class DisconnectEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.Disconnect();
        }
    }
}
