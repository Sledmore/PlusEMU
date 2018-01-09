namespace Plus.Communication.Packets.Outgoing.Rooms.AI.Pets
{
    class AddExperiencePointsComposer : ServerPacket
    {
        public AddExperiencePointsComposer(int PetId, int VirtualId, int Amount)
            : base(ServerPacketHeader.AddExperiencePointsMessageComposer)
        {
            WriteInteger(PetId);
            WriteInteger(VirtualId);
            WriteInteger(Amount);
        }
    }
}