using System;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Rooms.AI;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.AI.Pets
{
    class PetInformationComposer : MessageComposer
    {
        public Pet Pet { get; }
        public Habbo Habbo { get; }

        public PetInformationComposer(Pet Pet)
            : base(ServerPacketHeader.PetInformationMessageComposer)
        {
            this.Pet = Pet;
        }

        public PetInformationComposer(Habbo Habbo)
            : base(ServerPacketHeader.PetInformationMessageComposer)
        {
            this.Habbo = Habbo;
        }

        public override void Compose(ServerPacket packet)
        {
            if(Pet != null)
            {
                packet.WriteInteger(Pet.PetId);
                packet.WriteString(Pet.Name);
                packet.WriteInteger(Pet.Level);
                packet.WriteInteger(Pet.MaxLevel);
                packet.WriteInteger(Pet.experience);
                packet.WriteInteger(Pet.ExperienceGoal);
                packet.WriteInteger(Pet.Energy);
                packet.WriteInteger(Pet.MaxEnergy);
                packet.WriteInteger(Pet.Nutrition);
                packet.WriteInteger(Pet.MaxNutrition);
                packet.WriteInteger(Pet.Respect);
                packet.WriteInteger(Pet.OwnerId);
                packet.WriteInteger(Pet.Age);
                packet.WriteString(Pet.OwnerName);
                packet.WriteInteger(1);//3 on hab
                packet.WriteBoolean(Pet.Saddle > 0);
                packet.WriteBoolean(false);
                packet.WriteInteger(0);//5 on hab
                packet.WriteInteger(Pet.AnyoneCanRide); // Anyone can ride horse
                packet.WriteInteger(0);
                packet.WriteInteger(0);//512 on hab
                packet.WriteInteger(0);//1536
                packet.WriteInteger(0);//2560
                packet.WriteInteger(0);//3584
                packet.WriteInteger(0);
                packet.WriteString("");
                packet.WriteBoolean(false);
                packet.WriteInteger(-1);//255 on hab
                packet.WriteInteger(-1);
                packet.WriteInteger(-1);
                packet.WriteBoolean(false);
            } else if (Habbo != null)
            {
                packet.WriteInteger(Habbo.Id);
                packet.WriteString(Habbo.Username);
                packet.WriteInteger(Habbo.Rank);
                packet.WriteInteger(10);
                packet.WriteInteger(0);
                packet.WriteInteger(0);
                packet.WriteInteger(100);
                packet.WriteInteger(100);
                packet.WriteInteger(100);
                packet.WriteInteger(100);
                packet.WriteInteger(Habbo.GetStats().Respect);
                packet.WriteInteger(Habbo.Id);
                packet.WriteInteger(Convert.ToInt32(Math.Floor((PlusEnvironment.GetUnixTimestamp() - Habbo.AccountCreated) / 86400)));//How?
                packet.WriteString(Habbo.Username);
                packet.WriteInteger(1);//3 on hab
                packet.WriteBoolean(false);
                packet.WriteBoolean(false);
                packet.WriteInteger(0);//5 on hab
                packet.WriteInteger(0); // Anyone can ride horse
                packet.WriteInteger(0);
                packet.WriteInteger(0);//512 on hab
                packet.WriteInteger(0);//1536
                packet.WriteInteger(0);//2560
                packet.WriteInteger(0);//3584
                packet.WriteInteger(0);
                packet.WriteString("");
                packet.WriteBoolean(false);
                packet.WriteInteger(-1);//255 on hab
                packet.WriteInteger(-1);
                packet.WriteInteger(-1);
                packet.WriteBoolean(false);
            }
        }
    }
}
