namespace Plus.Communication.Packets.Outgoing.Inventory.Trading
{
    class TradingStartComposer : MessageComposer
    {
        public int User1Id { get; }
        public int User2Id { get; }

        public TradingStartComposer(int User1Id, int User2Id)
            : base(ServerPacketHeader.TradingStartMessageComposer)
        {
            this.User1Id = User1Id;
            this.User2Id = User2Id;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(User1Id);
            packet.WriteInteger(1);
            packet.WriteInteger(User2Id);
            packet.WriteInteger(1);
        }
    }
}
