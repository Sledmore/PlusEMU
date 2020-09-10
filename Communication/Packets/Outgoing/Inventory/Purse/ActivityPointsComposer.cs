namespace Plus.Communication.Packets.Outgoing.Inventory.Purse
{
    class ActivityPointsComposer : MessageComposer
    {
        public int PixelsBalance { get; }
        public int SeasonalCurrency { get; }
        public int GOTWPoints { get; }

        public ActivityPointsComposer(int PixelsBalance, int SeasonalCurrency, int GOTWPoints)
            : base(ServerPacketHeader.ActivityPointsMessageComposer)
        {
            this.PixelsBalance = PixelsBalance;
            this.SeasonalCurrency = SeasonalCurrency;
            this.GOTWPoints = GOTWPoints;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(11);//Count
            {
                packet.WriteInteger(0);//Pixels
                packet.WriteInteger(PixelsBalance);
                packet.WriteInteger(1);//Snowflakes
                packet.WriteInteger(16);
                packet.WriteInteger(2);//Hearts
                packet.WriteInteger(15);
                packet.WriteInteger(3);//Gift points
                packet.WriteInteger(14);
                packet.WriteInteger(4);//Shells
                packet.WriteInteger(13);
                packet.WriteInteger(5);//Diamonds
                packet.WriteInteger(SeasonalCurrency);
                packet.WriteInteger(101);//Snowflakes
                packet.WriteInteger(10);
                packet.WriteInteger(102);
                packet.WriteInteger(0);
                packet.WriteInteger(103);//Stars
                packet.WriteInteger(GOTWPoints);
                packet.WriteInteger(104);//Clouds
                packet.WriteInteger(0);
                packet.WriteInteger(105);//Diamonds
                packet.WriteInteger(0);
            }
        }
    }
}
