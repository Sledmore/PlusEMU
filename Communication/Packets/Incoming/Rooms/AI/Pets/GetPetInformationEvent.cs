using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Rooms.AI.Pets;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.AI.Pets
{
    class GetPetInformationEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            int petId = packet.PopInt();

            if (!session.GetHabbo().CurrentRoom.GetRoomUserManager().TryGetPet(petId, out RoomUser pet))
            {
                //Okay so, we've established we have no pets in this room by this virtual Id, let us check out users, maybe they're creeping as a pet?!
                RoomUser user = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(petId);
                if (user == null)
                    return;

                //Check some values first, please!
                if (user.GetClient() == null || user.GetClient().GetHabbo() == null)
                    return;

                //And boom! Let us send the information composer 8-).
                session.SendPacket(new PetInformationComposer(user.GetClient().GetHabbo()));
                return;
            }

            //Continue as a regular pet..
            if (pet.RoomId != session.GetHabbo().CurrentRoomId || pet.PetData == null)
                return;

            session.SendPacket(new PetInformationComposer(pet.PetData));
        }
    }
}
