using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.AI.Pets
{
    class PetHorseFigureInformationComposer : MessageComposer
    {
        public RoomUser PetUser { get; }

        public PetHorseFigureInformationComposer(RoomUser PetUser)
            : base(ServerPacketHeader.PetHorseFigureInformationMessageComposer)
        {
            this.PetUser = PetUser;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(PetUser.PetData.VirtualId);
            packet.WriteInteger(PetUser.PetData.PetId);
            packet.WriteInteger(PetUser.PetData.Type);
            packet.WriteInteger(int.Parse(PetUser.PetData.Race));
            packet.WriteString(PetUser.PetData.Color.ToLower());
            if (PetUser.PetData.Saddle > 0)
            {
                packet.WriteInteger(4);
                packet.WriteInteger(3);
                packet.WriteInteger(3);
                packet.WriteInteger(PetUser.PetData.PetHair);
                packet.WriteInteger(PetUser.PetData.HairDye);
                packet.WriteInteger(2);
                packet.WriteInteger(PetUser.PetData.PetHair);
                packet.WriteInteger(PetUser.PetData.HairDye);
                packet.WriteInteger(4);
                packet.WriteInteger(PetUser.PetData.Saddle);
                packet.WriteInteger(0);
            }
            else
            {
                packet.WriteInteger(1);
                packet.WriteInteger(2);
                packet.WriteInteger(2);
                packet.WriteInteger(PetUser.PetData.PetHair);
                packet.WriteInteger(PetUser.PetData.HairDye);
                packet.WriteInteger(3);
                packet.WriteInteger(PetUser.PetData.PetHair);
                packet.WriteInteger(PetUser.PetData.HairDye);
            }
            packet.WriteBoolean(PetUser.PetData.Saddle > 0);
            packet.WriteBoolean(PetUser.RidingHorse);
        }
    }
}
