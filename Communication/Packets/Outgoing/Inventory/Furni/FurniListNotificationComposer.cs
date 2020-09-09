namespace Plus.Communication.Packets.Outgoing.Inventory.Furni
{
    class FurniListNotificationComposer : MessageComposer
    {
        public int FurniId { get; }
        public int Type { get; }

        public FurniListNotificationComposer(int Id, int Type)
            : base(ServerPacketHeader.FurniListNotificationMessageComposer)
        {
            this.FurniId = Id;
            this.Type = Type;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(1);
            packet.WriteInteger(Type);
            packet.WriteInteger(1);
            packet.WriteInteger(Id);
        }
    }
}