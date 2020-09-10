namespace Plus.Communication.Packets.Outgoing.Marketplace
{
    class MarketplaceCanMakeOfferResultComposer : MessageComposer
    {
        public int Result { get; }

        public MarketplaceCanMakeOfferResultComposer(int Result)
            : base(ServerPacketHeader.MarketplaceCanMakeOfferResultMessageComposer)
        {
            this.Result = Result;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Result);
            packet.WriteInteger(0);
        }
    }
}
