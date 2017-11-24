using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Inventory.Purse
{
    class ActivityPointsComposer : ServerPacket
    {
        public ActivityPointsComposer(int PixelsBalance, int SeasionalCurrency, int GOTWPoints)
            : base(ServerPacketHeader.ActivityPointsMessageComposer)
        {
            base.WriteInteger(11);//Count
            {
                base.WriteInteger(0);//Pixels
                base.WriteInteger(PixelsBalance);
                base.WriteInteger(1);//Snowflakes
                base.WriteInteger(16);
                base.WriteInteger(2);//Hearts
                base.WriteInteger(15);
                base.WriteInteger(3);//Gift points
                base.WriteInteger(14);
                base.WriteInteger(4);//Shells
                base.WriteInteger(13);
                base.WriteInteger(5);//Diamonds
                base.WriteInteger(SeasionalCurrency);
                base.WriteInteger(101);//Snowflakes
                base.WriteInteger(10);
                base.WriteInteger(102);
                base.WriteInteger(0);
                base.WriteInteger(103);//Stars
                base.WriteInteger(GOTWPoints);
                base.WriteInteger(104);//Clouds
                base.WriteInteger(0);
                base.WriteInteger(105);//Diamonds
                base.WriteInteger(0);
            }
        }
    }
}
