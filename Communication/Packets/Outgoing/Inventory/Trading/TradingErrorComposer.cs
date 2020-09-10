namespace Plus.Communication.Packets.Outgoing.Inventory.Trading
{
    class TradingErrorComposer : MessageComposer
    {
        public int Error { get; }
        public string Username { get; }

        public TradingErrorComposer(int Error, string Username)
            : base(ServerPacketHeader.TradingErrorMessageComposer)
        {
            this.Error = Error;
            this.Username = Username;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Error);
            packet.WriteString(Username);
        }
    }
}
