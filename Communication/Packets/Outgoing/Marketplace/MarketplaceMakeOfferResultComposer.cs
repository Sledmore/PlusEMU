namespace Plus.Communication.Packets.Outgoing.Marketplace
{
    class MarketplaceMakeOfferResultComposer : MessageComposer
    {
        public int Success { get; }

        public MarketplaceMakeOfferResultComposer(int Success)
            : base(ServerPacketHeader.MarketplaceMakeOfferResultMessageComposer)
        {
            this.Success = Success;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Success);
        }
    }
}
