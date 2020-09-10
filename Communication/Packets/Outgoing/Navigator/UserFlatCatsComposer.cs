using System.Collections.Generic;

using Plus.HabboHotel.Navigator;

namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class UserFlatCatsComposer : MessageComposer
    {
        public ICollection<SearchResultList> Categories { get; }
        public int Rank { get; }

        public UserFlatCatsComposer(ICollection<SearchResultList> categories, int rank)
            : base(ServerPacketHeader.UserFlatCatsMessageComposer)
        {
            this.Categories = categories;
            this.Rank = rank;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Categories.Count);
            foreach (SearchResultList category in Categories)
            {
                packet.WriteInteger(category.Id);
                packet.WriteString(category.PublicName);
                packet.WriteBoolean(category.RequiredRank <= Rank);
                packet.WriteBoolean(false);
                packet.WriteString(string.Empty);
                packet.WriteString(string.Empty);
                packet.WriteBoolean(false);
            }
        }
    }
}