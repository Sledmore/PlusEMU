namespace Plus.Communication.Packets.Outgoing.Inventory.Purse
{
    class CreditBalanceComposer : ServerPacket
    {
        public CreditBalanceComposer(int creditsBalance)
            : base(ServerPacketHeader.CreditBalanceMessageComposer)
        {
           base.WriteString(creditsBalance + ".0");
        }
    }
}
