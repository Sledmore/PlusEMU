namespace Plus.Communication.Packets.Outgoing.Rooms.Settings
{
    class RoomSettingsSavedComposer : MessageComposer
    {
        public int RoomId { get; }
        public RoomSettingsSavedComposer(int roomID)
            : base(ServerPacketHeader.RoomSettingsSavedMessageComposer)
        {
            this.RoomId = roomID;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(RoomId);
        }
    }
}