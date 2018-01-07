namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{

    class RoomEntryInfoComposer : ServerPacket
    {
        public RoomEntryInfoComposer(int roomID, bool isOwner)
            : base(ServerPacketHeader.RoomEntryInfoMessageComposer)
        {
            base.WriteInteger(roomID);
            base.WriteBoolean(isOwner);
        }
    }
}
