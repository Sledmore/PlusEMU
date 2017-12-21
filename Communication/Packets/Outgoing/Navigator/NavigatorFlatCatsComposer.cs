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
            base.WriteInteger(categories.Count);
            foreach (SearchResultList category in categories.ToList())
            {
                base.WriteInteger(category.Id);
                base.WriteString(category.PublicName);
                base.WriteBoolean(true); // TODO
            }
        }
    }
}