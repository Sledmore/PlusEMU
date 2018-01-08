using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    class GetCatalogModeEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            // string mode = packet.PopString();
        }
    }
}
