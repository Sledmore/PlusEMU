namespace Plus.Communication.Packets.Outgoing.Rooms.Avatar
{
    class CarryObjectComposer : MessageComposer
    {
        public int VirtualId { get; }
        public int ItemId { get; }

        public CarryObjectComposer(int virtualID, int itemID)
            : base(ServerPacketHeader.CarryObjectMessageComposer)
        {
            this.VirtualId = virtualID;
            this.ItemId = itemID;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(VirtualId);
            packet.WriteInteger(ItemId);
        }
    }
}
