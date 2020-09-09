namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{

    class RoomEntryInfoComposer : MessageComposer
    {
        public int RoomId { get; }
        public bool IsOwner { get; }

        public RoomEntryInfoComposer(int roomID, bool isOwner)
            : base(ServerPacketHeader.RoomEntryInfoMessageComposer)
        {
            this.RoomId = roomID;
            this.IsOwner = isOwner;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(RoomId);
            packet.WriteBoolean(IsOwner);
        }
    }
}
