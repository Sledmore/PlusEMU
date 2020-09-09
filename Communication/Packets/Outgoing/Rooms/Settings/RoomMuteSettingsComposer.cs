namespace Plus.Communication.Packets.Outgoing.Rooms.Settings
{
    class RoomMuteSettingsComposer : MessageComposer
    {
        public bool Status { get; }
        public RoomMuteSettingsComposer(bool Status)
            : base(ServerPacketHeader.RoomMuteSettingsMessageComposer)
        {
            this.Status = Status;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteBoolean(Status);
        }
    }
}