namespace Plus.Communication.Packets.Outgoing.Rooms.FloorPlan
{
    class FloorPlanSendDoorComposer : ServerPacket
    {
        public FloorPlanSendDoorComposer(int DoorX, int DoorY, int DoorDirection)
            : base(ServerPacketHeader.FloorPlanSendDoorMessageComposer)
        {
            WriteInteger(DoorX);
            WriteInteger(DoorY);
            WriteInteger(DoorDirection);
        }
    }
}
