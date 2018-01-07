using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.Communication.Packets.Outgoing.BuildersClub;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    public class GetCatalogIndexEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.SendPacket(new CatalogIndexComposer(session, PlusEnvironment.GetGame().GetCatalog().GetPages()));
            session.SendPacket(new CatalogItemDiscountComposer());
            session.SendPacket(new BCBorrowedItemsComposer());
        }
    }
}