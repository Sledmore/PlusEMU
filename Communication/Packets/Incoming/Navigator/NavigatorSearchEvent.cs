using System.Collections.Generic;
using Plus.Communication.Packets.Outgoing.Navigator.New;
using Plus.HabboHotel.Navigator;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    class NavigatorSearchEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            string Category = packet.PopString();
            string Search = packet.PopString();

            ICollection<SearchResultList> Categories = new List<SearchResultList>();

            if (!string.IsNullOrEmpty(Search))
            {
                SearchResultList QueryResult = null;
                if (PlusEnvironment.GetGame().GetNavigator().TryGetSearchResultList(0, out QueryResult))
                {
                    Categories.Add(QueryResult);
                }
            }
            else
            {
                Categories = PlusEnvironment.GetGame().GetNavigator().GetCategorysForSearch(Category);
                if (Categories.Count == 0)
                {
                    //Are we going in deep?!
                    Categories = PlusEnvironment.GetGame().GetNavigator().GetResultByIdentifier(Category);
                    if (Categories.Count > 0)
                    {
                        session.SendPacket(new NavigatorSearchResultSetComposer(Category, Search, Categories, session, 2, 100));
                        return;
                    }
                }
            }

            session.SendPacket(new NavigatorSearchResultSetComposer(Category, Search, Categories, session));
        }
    }
}
