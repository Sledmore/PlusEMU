using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

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
            base.WriteInteger(Pet.VirtualId);
            base.WriteInteger(Pet.VirtualId);
            base.WriteInteger(Pet.PetId);//Pet Id, 100%
           base.WriteString(Pet.Name);
            base.WriteInteger(0);
            base.WriteInteger(0);
           base.WriteString(Pet.Color);
            base.WriteInteger(0);
            base.WriteInteger(0);//Count - 3 ints.
            base.WriteInteger(1);
        }

        public RespectPetNotificationMessageComposer(Habbo Habbo, RoomUser User)
            : base(ServerPacketHeader.RespectPetNotificationMessageComposer)
        {
            //TODO: Structure
            base.WriteInteger(User.VirtualId);
            base.WriteInteger(User.VirtualId);
            base.WriteInteger(Habbo.Id);//Pet Id, 100%
           base.WriteString(Habbo.Username);
            base.WriteInteger(0);
            base.WriteInteger(0);
           base.WriteString("FFFFFF");//Yeah..
            base.WriteInteger(0);
            base.WriteInteger(0);//Count - 3 ints.
            base.WriteInteger(1);
        }
    }
}
