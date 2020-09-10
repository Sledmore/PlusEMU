namespace Plus.Communication.Packets.Outgoing.Pets
{
    class PetBreedingComposer : MessageComposer
    {
        public PetBreedingComposer()
            : base(ServerPacketHeader.PetBreedingMessageComposer)
        {
           
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(219005779);//An Id?
            {
                //Pet 1.
                packet.WriteInteger(2169464);//Pet Id
                packet.WriteString("Tes");
                packet.WriteInteger(69);//Level
                packet.WriteString("1 22 F2E5CC");//Breed/figure?
                packet.WriteString("Sledmore");//Owner

                //Pet 2.
                packet.WriteInteger(2169465);
                packet.WriteString("Testy");
                packet.WriteInteger(1337);
                packet.WriteString("1 0 D4D4D4");
                packet.WriteString("Sledmore");

                packet.WriteInteger(4);//Count
                {
                    packet.WriteInteger(1);
                    packet.WriteInteger(3);
                    packet.WriteInteger(18);
                    packet.WriteInteger(19);
                    packet.WriteInteger(20);
                    packet.WriteInteger(3);
                    packet.WriteInteger(6);
                    packet.WriteInteger(12);
                    packet.WriteInteger(13);
                    packet.WriteInteger(14);
                    packet.WriteInteger(15);
                    packet.WriteInteger(16);
                    packet.WriteInteger(17);
                    packet.WriteInteger(4);
                    packet.WriteInteger(5);
                    packet.WriteInteger(7);
                    packet.WriteInteger(8);
                    packet.WriteInteger(9);
                    packet.WriteInteger(10);
                    packet.WriteInteger(11);
                    packet.WriteInteger(92);
                    packet.WriteInteger(6);
                    packet.WriteInteger(1);
                    packet.WriteInteger(2);
                    packet.WriteInteger(3);
                    packet.WriteInteger(4);
                    packet.WriteInteger(5);
                    packet.WriteInteger(6);
                }
                packet.WriteInteger(28);
            }
        }
    }
}