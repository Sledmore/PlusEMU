using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms.AI;

namespace Plus.Communication.Packets.Outgoing.Inventory.Pets
{
    class PetInventoryComposer : MessageComposer
    {
        public ICollection<Pet> Pets { get; }

        public PetInventoryComposer(ICollection<Pet> Pets)
            : base(ServerPacketHeader.PetInventoryMessageComposer)
        {
            this.Pets = Pets;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(1);
            packet.WriteInteger(1);
            packet.WriteInteger(Pets.Count);
            foreach (Pet Pet in Pets.ToList())
            {
                packet.WriteInteger(Pet.PetId);
                packet.WriteString(Pet.Name);
                packet.WriteInteger(Pet.Type);
                packet.WriteInteger(int.Parse(Pet.Race));
                packet.WriteString(Pet.Color);
                packet.WriteInteger(0);
                packet.WriteInteger(0);
                packet.WriteInteger(0);
            }
        }
    }
}