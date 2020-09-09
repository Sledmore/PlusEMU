namespace Plus.Communication.Packets.Outgoing.Inventory.Trading
{
    class TradingClosedComposer : MessageComposer
    {
        public int UserId { get; }

        public TradingClosedComposer(int UserId)
            : base(ServerPacketHeader.TradingClosedMessageComposer)
        {
            this.UserId = UserId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(UserId);
            packet.WriteInteger(0);
        }
    }
}
