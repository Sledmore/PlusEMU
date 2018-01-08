using System.Collections.Generic;

using Plus.HabboHotel.Rooms.AI;
using Plus.Communication.Packets.Outgoing.Inventory.Pets;

namespace Plus.Communication.Packets.Incoming.Inventory.Pets
{
    class GetPetInventoryEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().GetInventoryComponent() == null)
                return;

            ICollection<Pet> Pets = Session.GetHabbo().GetInventoryComponent().GetPets();
            Session.SendPacket(new PetInventoryComposer(Pets));
        }
    }
}