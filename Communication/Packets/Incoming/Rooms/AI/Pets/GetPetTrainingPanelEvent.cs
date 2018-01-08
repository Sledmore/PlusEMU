using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Rooms.AI.Pets;

namespace Plus.Communication.Packets.Incoming.Rooms.AI.Pets
{
    class GetPetTrainingPanelEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            int PetId = Packet.PopInt();

            RoomUser Pet = null;
            if (!Session.GetHabbo().CurrentRoom.GetRoomUserManager().TryGetPet(PetId, out Pet))
            {
                //Okay so, we've established we have no pets in this room by this virtual Id, let us check out users, maybe they're creeping as a pet?!
                RoomUser User = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(PetId);
                if (User == null)
                    return;

                //Check some values first, please!
                if (User.GetClient() == null || User.GetClient().GetHabbo() == null)
                    return;

                //And boom! Let us send the training panel composer 8-).
                Session.SendWhisper("Maybe one day, boo boo.");
                return;
            }

            //Continue as a regular pet..
            if (Pet.RoomId != Session.GetHabbo().CurrentRoomId || Pet.PetData == null)
                return;

            Session.SendPacket(new PetTrainingPanelComposer(Pet.PetData.PetId, Pet.PetData.Level));
        }
    }
}
