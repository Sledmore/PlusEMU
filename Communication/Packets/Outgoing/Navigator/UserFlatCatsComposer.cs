using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Navigator;

namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class UserFlatCatsComposer : ServerPacket
    {
        public UserFlatCatsComposer(ICollection<SearchResultList> Categories, int Rank)
            : base(ServerPacketHeader.UserFlatCatsMessageComposer)
        {
            base.WriteInteger(Categories.Count);
            foreach (SearchResultList Cat in Categories)
            {
                base.WriteInteger(Cat.Id);
                base.WriteString(Cat.PublicName);
                base.WriteBoolean(Cat.RequiredRank <= Rank);
                base.WriteBoolean(false);
                base.WriteString("");
                base.WriteString("");
                base.WriteBoolean(false);
            }
        }
    }
}