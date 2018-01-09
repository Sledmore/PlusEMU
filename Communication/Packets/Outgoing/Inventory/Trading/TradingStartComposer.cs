namespace Plus.Communication.Packets.Outgoing.Inventory.Trading
{
    class TradingStartComposer : ServerPacket
    {
        public TradingStartComposer(int User1Id, int User2Id)
            : base(ServerPacketHeader.TradingStartMessageComposer)
        {
            WriteInteger(User1Id);
            WriteInteger(1);
            WriteInteger(User2Id);
            WriteInteger(1);
        }
    }
}
