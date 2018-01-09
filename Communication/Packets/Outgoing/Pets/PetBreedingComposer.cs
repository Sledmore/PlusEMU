namespace Plus.Communication.Packets.Outgoing.Pets
{
    class PetBreedingComposer : ServerPacket
    {
        public PetBreedingComposer()
            : base(ServerPacketHeader.PetBreedingMessageComposer)
        {
            WriteInteger(219005779);//An Id?
            {
                //Pet 1.
                WriteInteger(2169464);//Pet Id
                WriteString("Tes");
                WriteInteger(69);//Level
                WriteString("1 22 F2E5CC");//Breed/figure?
                WriteString("Sledmore");//Owner

                //Pet 2.
                WriteInteger(2169465);
                WriteString("Testy");
                WriteInteger(1337);
                WriteString("1 0 D4D4D4");
                WriteString("Sledmore");

                WriteInteger(4);//Count
                {
                    WriteInteger(1);
                    WriteInteger(3);
                    WriteInteger(18);
                    WriteInteger(19);
                    WriteInteger(20);
                    WriteInteger(3);
                    WriteInteger(6);
                    WriteInteger(12);
                    WriteInteger(13);
                    WriteInteger(14);
                    WriteInteger(15);
                    WriteInteger(16);
                    WriteInteger(17);
                    WriteInteger(4);
                    WriteInteger(5);
                    WriteInteger(7);
                    WriteInteger(8);
                    WriteInteger(9);
                    WriteInteger(10);
                    WriteInteger(11);
                    WriteInteger(92);
                    WriteInteger(6);
                    WriteInteger(1);
                    WriteInteger(2);
                    WriteInteger(3);
                    WriteInteger(4);
                    WriteInteger(5);
                    WriteInteger(6);
                }
                WriteInteger(28);
            }
        }
    }
}