using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.AI.Pets
{
    class PetHorseFigureInformationComposer : ServerPacket
    {
        public PetHorseFigureInformationComposer(RoomUser PetUser)
            : base(ServerPacketHeader.PetHorseFigureInformationMessageComposer)
        {
            base.WriteInteger(PetUser.PetData.VirtualId);
            base.WriteInteger(PetUser.PetData.PetId);
            base.WriteInteger(PetUser.PetData.Type);
            base.WriteInteger(int.Parse(PetUser.PetData.Race));
           base.WriteString(PetUser.PetData.Color.ToLower());
            if (PetUser.PetData.Saddle > 0)
            {
                base.WriteInteger(4);
                base.WriteInteger(3);
                base.WriteInteger(3);
                base.WriteInteger(PetUser.PetData.PetHair);
                base.WriteInteger(PetUser.PetData.HairDye);
                base.WriteInteger(2);
                base.WriteInteger(PetUser.PetData.PetHair);
                base.WriteInteger(PetUser.PetData.HairDye);
                base.WriteInteger(4);
                base.WriteInteger(PetUser.PetData.Saddle);
                base.WriteInteger(0);
            }
            else
            {
                base.WriteInteger(1);
                base.WriteInteger(2);
                base.WriteInteger(2);
                base.WriteInteger(PetUser.PetData.PetHair);
                base.WriteInteger(PetUser.PetData.HairDye);
                base.WriteInteger(3);
                base.WriteInteger(PetUser.PetData.PetHair);
                base.WriteInteger(PetUser.PetData.HairDye);
            }
            base.WriteBoolean(PetUser.PetData.Saddle > 0);
            base.WriteBoolean(PetUser.RidingHorse);
        }
    }
}
