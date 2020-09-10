namespace Plus.Communication.Packets.Outgoing.Inventory.Furni
{
    class FurniListRemoveComposer : MessageComposer
    {
        public int FurniId { get; }

        public FurniListRemoveComposer(int Id)
            : base(ServerPacketHeader.FurniListRemoveMessageComposer)
        {
            this.FurniId = Id;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(FurniId);
        }
    }
}
