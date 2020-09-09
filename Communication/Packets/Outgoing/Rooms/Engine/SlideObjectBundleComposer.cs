namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class SlideObjectBundleComposer : MessageComposer
    {
        public int FromX { get; }
        public int FromY { get; }
        public double FromZ { get; }
        public int ToX { get; }
        public int ToY { get; }
        public double ToZ { get; }
        public int RollerId { get; }
        public int AvatarId { get; }
        public int ItemId { get; }

        public SlideObjectBundleComposer(int FromX, int FromY, double FromZ, int ToX, int ToY, double ToZ, int RollerId, int AvatarId, int ItemId)
            : base(ServerPacketHeader.SlideObjectBundleMessageComposer)
        {
            this.FromX = FromX;
            this.FromY = FromY;
            this.FromZ = FromZ;
            this.ToX = ToX;
            this.ToY = ToY;
            this.ToZ = ToZ;
            this.RollerId = RollerId;
            this.AvatarId = AvatarId;
            this.ItemId = ItemId;
        }

        public override void Compose(ServerPacket packet)
        {
            bool IsItem = ItemId > 0;

            packet.WriteInteger(FromX);
            packet.WriteInteger(FromY);
            packet.WriteInteger(ToX);
            packet.WriteInteger(ToY);
            packet.WriteInteger(IsItem ? 1 : 0);

            if (IsItem)
                packet.WriteInteger(ItemId);
            else
            {
                packet.WriteInteger(RollerId);
                packet.WriteInteger(2);
                packet.WriteInteger(AvatarId);
            }

            packet.WriteDouble(FromZ);
            packet.WriteDouble(ToZ);

            if (IsItem)
            {
                packet.WriteInteger(RollerId);
            }
        }
    }
}
