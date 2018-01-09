using System.Linq;
using System.Collections.Generic;

using Plus.Utilities;
using Plus.HabboHotel.Users.Messenger;

using Plus.Communication.Packets.Outgoing.Messenger;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Messenger
{
    class HabboSearchEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || session.GetHabbo().GetMessenger() == null)
                return;

            string query = StringCharFilter.Escape(packet.PopString().Replace("%", ""));
            if (query.Length < 1 || query.Length > 100)
                return;

            List<SearchResult> friends = new List<SearchResult>();
            List<SearchResult> othersUsers = new List<SearchResult>();

            List<SearchResult> results = SearchResultFactory.GetSearchResult(query);
            foreach (SearchResult result in results.ToList())
            {
                if (session.GetHabbo().GetMessenger().FriendshipExists(result.UserId))
                    friends.Add(result);
                else
                    othersUsers.Add(result);
            }

            session.SendPacket(new HabboSearchResultComposer(friends, othersUsers));
        }
    }
}