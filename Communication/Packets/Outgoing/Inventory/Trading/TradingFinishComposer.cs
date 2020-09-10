namespace Plus.Communication.Packets.Outgoing.Inventory.Trading
{
    class TradingFinishComposer : MessageComposer
    {
        public TradingFinishComposer()
            : base(ServerPacketHeader.TradingFinishMessageComposer)
        {
        }

        public override void Compose(ServerPacket packet)
        {
            
        }
    }
}
