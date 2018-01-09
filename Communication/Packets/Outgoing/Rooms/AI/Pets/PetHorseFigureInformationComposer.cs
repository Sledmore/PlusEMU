using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.AI.Pets
{
    class PetHorseFigureInformationComposer : ServerPacket
    {
        public PetHorseFigureInformationComposer(RoomUser PetUser)
            : base(ServerPacketHeader.PetHorseFigureInformationMessageComposer)
        {
            WriteInteger(PetUser.PetData.VirtualId);
            WriteInteger(PetUser.PetData.PetId);
            WriteInteger(PetUser.PetData.Type);
            WriteInteger(int.Parse(PetUser.PetData.Race));
           WriteString(PetUser.PetData.Color.ToLower());
            if (PetUser.PetData.Saddle > 0)
            {
                WriteInteger(4);
                WriteInteger(3);
                WriteInteger(3);
                WriteInteger(PetUser.PetData.PetHair);
                WriteInteger(PetUser.PetData.HairDye);
                WriteInteger(2);
                WriteInteger(PetUser.PetData.PetHair);
                WriteInteger(PetUser.PetData.HairDye);
                WriteInteger(4);
                WriteInteger(PetUser.PetData.Saddle);
                WriteInteger(0);
            }
            else
            {
                WriteInteger(1);
                WriteInteger(2);
                WriteInteger(2);
                WriteInteger(PetUser.PetData.PetHair);
                WriteInteger(PetUser.PetData.HairDye);
                WriteInteger(3);
                WriteInteger(PetUser.PetData.PetHair);
                WriteInteger(PetUser.PetData.HairDye);
            }
            WriteBoolean(PetUser.PetData.Saddle > 0);
            WriteBoolean(PetUser.RidingHorse);
        }
    }
}
