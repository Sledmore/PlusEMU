namespace Plus.Communication.Packets.Outgoing.Inventory.Purse
{
    class CreditBalanceComposer : MessageComposer
    {
        public int CreditsBalance { get; }

        public CreditBalanceComposer(int creditsBalance)
            : base(ServerPacketHeader.CreditBalanceMessageComposer)
        {
            this.CreditsBalance = creditsBalance;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(CreditsBalance + ".0");
        }
    }
}
