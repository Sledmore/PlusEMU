namespace Plus.Communication.Packets.Outgoing.Rooms.FloorPlan
{
    class FloorPlanSendDoorComposer : MessageComposer
    {
        public int DoorX { get; }
        public int DoorY { get; }
        public int DoorDirection { get; }

        public FloorPlanSendDoorComposer(int DoorX, int DoorY, int DoorDirection)
            : base(ServerPacketHeader.FloorPlanSendDoorMessageComposer)
        {
            this.DoorX = DoorX;
            this.DoorY = DoorY;
            this.DoorDirection = DoorDirection;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(DoorX);
            packet.WriteInteger(DoorY);
            packet.WriteInteger(DoorDirection);
        }
    }
}
