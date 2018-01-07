namespace Plus.Communication.Packets.Outgoing.Inventory.Trading
{
    class TradingErrorComposer : ServerPacket
    {
        public TradingErrorComposer(int Error, string Username)
            : base(ServerPacketHeader.TradingErrorMessageComposer)
        {
            base.WriteInteger(Error);
           base.WriteString(Username);
        }
    }
}
