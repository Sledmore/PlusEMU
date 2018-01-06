using Plus.HabboHotel.Navigator;
using System.Collections.Generic;
using System.Linq;

namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class NavigatorFlatCatsComposer : ServerPacket
    {
        public NavigatorFlatCatsComposer(ICollection<SearchResultList> categories)
            : base(ServerPacketHeader.NavigatorFlatCatsMessageComposer)
        {
            WriteInteger(categories.Count);
            foreach (SearchResultList category in categories.ToList())
            {
                WriteInteger(category.Id);
                WriteString(category.PublicName);
                WriteBoolean(true); // TODO
            }
        }
    }
}