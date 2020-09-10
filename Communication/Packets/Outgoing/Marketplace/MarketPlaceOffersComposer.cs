using System.Collections.Generic;

using Plus.HabboHotel.Catalog.Marketplace;

namespace Plus.Communication.Packets.Outgoing.Marketplace
{
    class MarketPlaceOffersComposer : MessageComposer
    {
        public Dictionary<int, MarketOffer> Offers { get; }
        public Dictionary<int, int> Dictionary2 { get; }

        public MarketPlaceOffersComposer(Dictionary<int, MarketOffer> dictionary, Dictionary<int, int> dictionary2)
            : base(ServerPacketHeader.MarketPlaceOffersMessageComposer)
        {
            this.Offers = dictionary;
            this.Dictionary2 = dictionary2;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Offers.Count);
            if (Offers.Count > 0)
            {
                foreach (KeyValuePair<int, MarketOffer> pair in Offers)
                {
                    packet.WriteInteger(pair.Value.OfferID);
                    packet.WriteInteger(1);//State
                    packet.WriteInteger(1);
                    packet.WriteInteger(pair.Value.SpriteId);

                    packet.WriteInteger(256);
                    packet.WriteString("");
                    packet.WriteInteger(pair.Value.LimitedNumber);
                    packet.WriteInteger(pair.Value.LimitedStack);

                    packet.WriteInteger(pair.Value.TotalPrice);
                    packet.WriteInteger(0);
                    packet.WriteInteger(PlusEnvironment.GetGame().GetCatalog().GetMarketplace().AvgPriceForSprite(pair.Value.SpriteId));
                    packet.WriteInteger(Dictionary2[pair.Value.SpriteId]);
                }
            }
            packet.WriteInteger(Offers.Count);//Item count to show how many were found.
        }
    }
}