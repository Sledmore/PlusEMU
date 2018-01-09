using System.Collections.Generic;
using Plus.HabboHotel.LandingView.Promotions;
using Plus.Communication.Packets.Outgoing.LandingView;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.LandingView
{
    class GetPromoArticlesEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            ICollection<Promotion> landingPromotions = PlusEnvironment.GetGame().GetLandingManager().GetPromotionItems();

            session.SendPacket(new PromoArticlesComposer(landingPromotions));
        }
    }
}
