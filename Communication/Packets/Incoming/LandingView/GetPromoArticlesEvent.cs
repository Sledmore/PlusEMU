using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.LandingView;
using Plus.HabboHotel.LandingView.Promotions;
using Plus.Communication.Packets.Outgoing.LandingView;

namespace Plus.Communication.Packets.Incoming.LandingView
{
    class GetPromoArticlesEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            ICollection<Promotion> LandingPromotions = PlusEnvironment.GetGame().GetLandingManager().GetPromotionItems();

            Session.SendPacket(new PromoArticlesComposer(LandingPromotions));
        }
    }
}
