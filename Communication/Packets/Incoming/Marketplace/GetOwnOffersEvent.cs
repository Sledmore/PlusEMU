using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Communication.Packets.Outgoing.Marketplace;

namespace Plus.Communication.Packets.Incoming.Marketplace
{
    class GetOwnOffersEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new MarketPlaceOwnOffersComposer(Session.GetHabbo().Id));
        }
    }
}
