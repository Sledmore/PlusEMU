using System.Linq;
using System.Collections.Generic;
using Plus.HabboHotel.LandingView.Promotions;


namespace Plus.Communication.Packets.Outgoing.LandingView
{
    class PromoArticlesComposer : MessageComposer
    {
        public ICollection<Promotion> LandingPromotions { get; }

        public PromoArticlesComposer(ICollection<Promotion> LandingPromotions)
            : base(ServerPacketHeader.PromoArticlesMessageComposer)
        {
            this.LandingPromotions = LandingPromotions;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(LandingPromotions.Count);//Count
            foreach (Promotion Promotion in LandingPromotions.ToList())
            {
                packet.WriteInteger(Promotion.Id); //ID
                packet.WriteString(Promotion.Title); //Title
                packet.WriteString(Promotion.Text); //Text
                packet.WriteString(Promotion.ButtonText); //Button text
                packet.WriteInteger(Promotion.ButtonType); //Link type 0 and 3
                packet.WriteString(Promotion.ButtonLink); //Link to article
                packet.WriteString(Promotion.ImageLink); //Image link
            }
        }
    }
}