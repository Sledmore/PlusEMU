namespace Plus.Communication.Packets.Outgoing.Inventory.Trading
{
    class TradingAcceptComposer : MessageComposer
    {
        public int UserId { get; }
        public bool Accept { get; }

        public TradingAcceptComposer(int UserId, bool Accept)
            : base(ServerPacketHeader.TradingAcceptMessageComposer)
        {
            this.UserId = UserId;
            this.Accept = Accept;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(UserId);
            packet.WriteInteger(Accept ? 1 : 0);
        }
    }
}
