using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Communication.Packets.Outgoing.Marketplace;

namespace Plus.Communication.Packets.Incoming.Marketplace
{
    class GetMarketplaceCanMakeOfferEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int ErrorCode = (Session.GetHabbo().TradingLockExpiry > 0 ? 6 : 1);

            Session.SendPacket(new MarketplaceCanMakeOfferResultComposer(ErrorCode));
        }
    }
}