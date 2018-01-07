using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    public class GetMarketplaceConfigurationEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.SendPacket(new MarketplaceConfigurationComposer());
        }
    }
}