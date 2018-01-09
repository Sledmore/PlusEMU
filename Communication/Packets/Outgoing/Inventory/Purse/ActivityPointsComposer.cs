namespace Plus.Communication.Packets.Outgoing.Inventory.Purse
{
    class ActivityPointsComposer : ServerPacket
    {
        public ActivityPointsComposer(int PixelsBalance, int SeasionalCurrency, int GOTWPoints)
            : base(ServerPacketHeader.ActivityPointsMessageComposer)
        {
            WriteInteger(11);//Count
            {
                WriteInteger(0);//Pixels
                WriteInteger(PixelsBalance);
                WriteInteger(1);//Snowflakes
                WriteInteger(16);
                WriteInteger(2);//Hearts
                WriteInteger(15);
                WriteInteger(3);//Gift points
                WriteInteger(14);
                WriteInteger(4);//Shells
                WriteInteger(13);
                WriteInteger(5);//Diamonds
                WriteInteger(SeasionalCurrency);
                WriteInteger(101);//Snowflakes
                WriteInteger(10);
                WriteInteger(102);
                WriteInteger(0);
                WriteInteger(103);//Stars
                WriteInteger(GOTWPoints);
                WriteInteger(104);//Clouds
                WriteInteger(0);
                WriteInteger(105);//Diamonds
                WriteInteger(0);
            }
        }
    }
}
