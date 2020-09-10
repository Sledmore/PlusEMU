using Plus.HabboHotel.Navigator;
using System.Collections.Generic;
using System.Linq;

namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class NavigatorFlatCatsComposer : MessageComposer
    {
        public ICollection<SearchResultList> Categories { get; }

        public NavigatorFlatCatsComposer(ICollection<SearchResultList> categories)
            : base(ServerPacketHeader.NavigatorFlatCatsMessageComposer)
        {
            Categories = categories;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Categories.Count);
            foreach (SearchResultList category in Categories.ToList())
            {
                packet.WriteInteger(category.Id);
                packet.WriteString(category.PublicName);
                packet.WriteBoolean(true); // TODO
            }
        }
    }
}