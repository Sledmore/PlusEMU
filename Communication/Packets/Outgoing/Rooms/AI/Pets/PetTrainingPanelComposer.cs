namespace Plus.Communication.Packets.Outgoing.Rooms.AI.Pets
{
    class PetTrainingPanelComposer : MessageComposer
    {
        public int PetId { get; }
        public int Level { get; }

        public PetTrainingPanelComposer(int PetId, int Level)
            : base(ServerPacketHeader.PetTrainingPanelMessageComposer)
        {
            this.PetId = PetId;
            this.Level = Level;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(PetId);//Pet Id for sure.

            //Commands available to be done.
            packet.WriteInteger(8);//Count
            {
                packet.WriteInteger(46);//Breed?
                packet.WriteInteger(0);//Command Id
                packet.WriteInteger(1);
                packet.WriteInteger(2);
                packet.WriteInteger(3);
                packet.WriteInteger(4);
                packet.WriteInteger(5);
                packet.WriteInteger(6);
            }

            //Commands that can be used NOW. (Level ups give you new commands etc).
            packet.WriteInteger(GetCount(Level));//Count
            {
                packet.WriteInteger(46);//Breed?
                packet.WriteInteger(0);//Command Id
                packet.WriteInteger(1);
                packet.WriteInteger(2);
                packet.WriteInteger(3);
                packet.WriteInteger(4);
                packet.WriteInteger(5);
                packet.WriteInteger(6);
            }
        }

        public int GetCount(int Level)
        {
            switch(Level)
            {
                case 1:
                case 2:
                    return 1;

                case 3:
                case 4:
                    return 2;

                case 5:
                case 6:
                    return 3;

                case 7:
                case 8:
                    return 4;

                case 9:
                case 10:
                    return 5;

                case 11:
                case 12:
                    return 6;

                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                    return 8;

                default:
                    return 1;
            }
        }
    }
}
