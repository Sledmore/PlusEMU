namespace Plus.Communication.Packets.Outgoing.Rooms.AI.Pets
{
    class AddExperiencePointsComposer : MessageComposer
    {
        public int PetId { get; }
        public int VirtualId { get; }
        public int Amount { get; }

        public AddExperiencePointsComposer(int PetId, int VirtualId, int Amount)
            : base(ServerPacketHeader.AddExperiencePointsMessageComposer)
        {
            this.PetId = PetId;
            this.VirtualId = VirtualId;
            this.Amount = Amount;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(PetId);
            packet.WriteInteger(VirtualId);
            packet.WriteInteger(Amount);
        }
    }
}