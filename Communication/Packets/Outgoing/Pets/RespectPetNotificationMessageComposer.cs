using Plus.HabboHotel.Users;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Rooms.AI;

namespace Plus.Communication.Packets.Outgoing.Pets
{
    class RespectPetNotificationMessageComposer : MessageComposer
    {
        public Pet Pet { get; }
        public RoomUser RoomUser { get; }

        public RespectPetNotificationMessageComposer(Pet Pet)
            : base(ServerPacketHeader.RespectPetNotificationMessageComposer)
        {
            this.Pet = Pet;
        }

        public RespectPetNotificationMessageComposer(RoomUser User)
            : base(ServerPacketHeader.RespectPetNotificationMessageComposer)
        {
            this.RoomUser = User;
        }

        public override void Compose(ServerPacket packet)
        {
            if(Pet != null)
            {
                //TODO: Structure
                packet.WriteInteger(Pet.VirtualId);
                packet.WriteInteger(Pet.VirtualId);
                packet.WriteInteger(Pet.PetId);//Pet Id, 100%
                packet.WriteString(Pet.Name);
                packet.WriteInteger(0);
                packet.WriteInteger(0);
                packet.WriteString(Pet.Color);
                packet.WriteInteger(0);
                packet.WriteInteger(0);//Count - 3 ints.
                packet.WriteInteger(1);
            } else if(RoomUser != null)
            {
                //TODO: Structure
                packet.WriteInteger(RoomUser.VirtualId);
                packet.WriteInteger(RoomUser.VirtualId);
                packet.WriteInteger(RoomUser.GetClient().GetHabbo().Id);//Pet Id, 100%
                packet.WriteString(RoomUser.GetClient().GetHabbo().Username);
                packet.WriteInteger(0);
                packet.WriteInteger(0);
                packet.WriteString("FFFFFF");//Yeah..
                packet.WriteInteger(0);
                packet.WriteInteger(0);//Count - 3 ints.
                packet.WriteInteger(1);
            }
        }
    }
}
