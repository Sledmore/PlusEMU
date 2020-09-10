namespace Plus.Communication.Packets.Outgoing.Inventory.Trading
{
    class TradingConfirmedComposer : MessageComposer
    {
        public int UserId { get; }
        public bool Confirmed { get; }

        public TradingConfirmedComposer(int UserId, bool Confirmed)
            : base(ServerPacketHeader.TradingConfirmedMessageComposer)
        {
            this.UserId = UserId;
            this.Confirmed = Confirmed;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(UserId);
            packet.WriteInteger(Confirmed ? 1 : 0);
        }
    }
}
