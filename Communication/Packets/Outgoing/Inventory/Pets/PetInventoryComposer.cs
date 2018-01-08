using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms.AI;

namespace Plus.Communication.Packets.Outgoing.Inventory.Pets
{
    class PetInventoryComposer : ServerPacket
    {
        public PetInventoryComposer(ICollection<Pet> Pets)
            : base(ServerPacketHeader.PetInventoryMessageComposer)
        {
            WriteInteger(1);
            WriteInteger(1);
            WriteInteger(Pets.Count);
            foreach (Pet Pet in Pets.ToList())
            {
                WriteInteger(Pet.PetId);
               WriteString(Pet.Name);
                WriteInteger(Pet.Type);
                WriteInteger(int.Parse(Pet.Race));
               WriteString(Pet.Color);
                WriteInteger(0);
                WriteInteger(0);
                WriteInteger(0);
            }
        }
    }
}