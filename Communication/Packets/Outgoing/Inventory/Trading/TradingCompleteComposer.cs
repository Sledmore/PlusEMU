namespace Plus.Communication.Packets.Outgoing.Inventory.Trading
{
    class TradingCompleteComposer : MessageComposer
    {
        public TradingCompleteComposer()
            : base(ServerPacketHeader.TradingCompleteMessageComposer)
        {
        }

        public override void Compose(ServerPacket packet)
        {
            
        }
    }
}
