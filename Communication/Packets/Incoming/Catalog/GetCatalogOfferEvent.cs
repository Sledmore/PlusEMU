using Plus.HabboHotel.Catalog;
using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    class GetCatalogOfferEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int offerId = packet.PopInt();
            if (!PlusEnvironment.GetGame().GetCatalog().ItemOffers.ContainsKey(offerId))
                return;

            int pageId = PlusEnvironment.GetGame().GetCatalog().ItemOffers[offerId];

            if (!PlusEnvironment.GetGame().GetCatalog().TryGetPage(pageId, out CatalogPage page))
                return;

            if (!page.Enabled || !page.Visible || page.MinimumRank > session.GetHabbo().Rank || (page.MinimumVIP > session.GetHabbo().VIPRank && session.GetHabbo().Rank == 1))
                return;

            if (!page.ItemOffers.ContainsKey(offerId))
                return;

            var item = page.ItemOffers[offerId];
            if (item != null)
                session.SendPacket(new CatalogOfferComposer(item));
        }
    }
}
