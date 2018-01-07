namespace Plus.Communication.Packets.Outgoing.Rooms.Settings
{
    class RoomMuteSettingsComposer : ServerPacket
    {
        public RoomMuteSettingsComposer(bool Status)
            : base(ServerPacketHeader.RoomMuteSettingsMessageComposer)
        {
            base.WriteBoolean(Status);
        }
    }
}