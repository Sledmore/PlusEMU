using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.HabboHotel.Catalog;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    public class GetCatalogPageEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int pageId = packet.PopInt();
            packet.PopInt();
            string cataMode = packet.PopString();

            if (!PlusEnvironment.GetGame().GetCatalog().TryGetPage(pageId, out CatalogPage page))
                return;

            if (!page.Enabled || !page.Visible || page.MinimumRank > session.GetHabbo().Rank || (page.MinimumVIP > session.GetHabbo().VIPRank && session.GetHabbo().Rank == 1))
                return;

           session.SendPacket(new CatalogPageComposer(page, cataMode));
        }
    }
}