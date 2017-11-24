using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Marketplace
{
    class MarketplaceCanMakeOfferResultComposer : ServerPacket
    {
        public MarketplaceCanMakeOfferResultComposer(int Result)
            : base(ServerPacketHeader.MarketplaceCanMakeOfferResultMessageComposer)
        {
            base.WriteInteger(Result);
            base.WriteInteger(0);
        }
    }
}
