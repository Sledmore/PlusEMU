using System.Collections.Generic;
using System.Linq;
using log4net.Core;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Navigator;
using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.Navigator.New
{
    class NavigatorSearchResultSetComposer : MessageComposer
    {
        public string Category { get; }
        public string Data { get; }
        public ICollection<SearchResultList> SearchResultLists { get; }
        public int GoBack { get; }
        public int FetchLimit { get; }
        public Habbo Habbo { get; }

        public NavigatorSearchResultSetComposer(string category, string data,
            ICollection<SearchResultList> searchResultLists, Habbo habbo, int goBack = 1, int fetchLimit = 12)
            : base(ServerPacketHeader.NavigatorSearchResultSetMessageComposer)
        {
            this.Category = category;
            this.Data = data;
            this.SearchResultLists = searchResultLists;
            this.Habbo = habbo;
            this.GoBack = goBack;
            this.FetchLimit = fetchLimit;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(Category); //Search code.
            packet.WriteString(Data); //Text?

            packet.WriteInteger(SearchResultLists.Count); //Count
            foreach (SearchResultList searchResult in SearchResultLists.ToList())
            {
                packet.WriteString(searchResult.CategoryIdentifier);
                packet.WriteString(searchResult.PublicName);
                packet.WriteInteger(NavigatorSearchAllowanceUtility.GetIntegerValue(searchResult.SearchAllowance) != 0
                    ? GoBack
                    : NavigatorSearchAllowanceUtility.GetIntegerValue(searchResult
                        .SearchAllowance)); //0 = nothing, 1 = show more, 2 = back Action allowed.
                packet.WriteBoolean(false); //True = minimized, false = open.
                packet.WriteInteger(searchResult.ViewMode == NavigatorViewMode.Regular ? 0 :
                    searchResult.ViewMode == NavigatorViewMode.Thumbnail ? 1 :
                    0); //View mode, 0 = tiny/regular, 1 = thumbnail

                NavigatorHandler.Search(packet, searchResult, Data, Habbo, FetchLimit);
            }
        }
    }
}
