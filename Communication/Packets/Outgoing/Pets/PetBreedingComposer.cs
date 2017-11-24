using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.Pets
{
    class PetBreedingComposer : ServerPacket
    {
        public PetBreedingComposer()
            : base(ServerPacketHeader.PetBreedingMessageComposer)
        {
            base.WriteInteger(219005779);//An Id?
            {
                //Pet 1.
                base.WriteInteger(2169464);//Pet Id
                base.WriteString("Tes");
                base.WriteInteger(69);//Level
                base.WriteString("1 22 F2E5CC");//Breed/figure?
                base.WriteString("Sledmore");//Owner

                //Pet 2.
                base.WriteInteger(2169465);
                base.WriteString("Testy");
                base.WriteInteger(1337);
                base.WriteString("1 0 D4D4D4");
                base.WriteString("Sledmore");

                base.WriteInteger(4);//Count
                {
                    base.WriteInteger(1);
                    base.WriteInteger(3);
                    base.WriteInteger(18);
                    base.WriteInteger(19);
                    base.WriteInteger(20);
                    base.WriteInteger(3);
                    base.WriteInteger(6);
                    base.WriteInteger(12);
                    base.WriteInteger(13);
                    base.WriteInteger(14);
                    base.WriteInteger(15);
                    base.WriteInteger(16);
                    base.WriteInteger(17);
                    base.WriteInteger(4);
                    base.WriteInteger(5);
                    base.WriteInteger(7);
                    base.WriteInteger(8);
                    base.WriteInteger(9);
                    base.WriteInteger(10);
                    base.WriteInteger(11);
                    base.WriteInteger(92);
                    base.WriteInteger(6);
                    base.WriteInteger(1);
                    base.WriteInteger(2);
                    base.WriteInteger(3);
                    base.WriteInteger(4);
                    base.WriteInteger(5);
                    base.WriteInteger(6);
                }
                base.WriteInteger(28);
            }
        }
    }
}