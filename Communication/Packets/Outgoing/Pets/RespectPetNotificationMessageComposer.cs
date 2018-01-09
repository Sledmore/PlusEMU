using Plus.HabboHotel.Users;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Rooms.AI;

namespace Plus.Communication.Packets.Outgoing.Pets
{
    class RespectPetNotificationMessageComposer : ServerPacket
    {
        public RespectPetNotificationMessageComposer(Pet Pet)
            : base(ServerPacketHeader.RespectPetNotificationMessageComposer)
        {
            //TODO: Structure
            WriteInteger(Pet.VirtualId);
            WriteInteger(Pet.VirtualId);
            WriteInteger(Pet.PetId);//Pet Id, 100%
           WriteString(Pet.Name);
            WriteInteger(0);
            WriteInteger(0);
           WriteString(Pet.Color);
            WriteInteger(0);
            WriteInteger(0);//Count - 3 ints.
            WriteInteger(1);
        }

        public RespectPetNotificationMessageComposer(Habbo Habbo, RoomUser User)
            : base(ServerPacketHeader.RespectPetNotificationMessageComposer)
        {
            //TODO: Structure
            WriteInteger(User.VirtualId);
            WriteInteger(User.VirtualId);
            WriteInteger(Habbo.Id);//Pet Id, 100%
           WriteString(Habbo.Username);
            WriteInteger(0);
            WriteInteger(0);
           WriteString("FFFFFF");//Yeah..
            WriteInteger(0);
            WriteInteger(0);//Count - 3 ints.
            WriteInteger(1);
        }
    }
}
