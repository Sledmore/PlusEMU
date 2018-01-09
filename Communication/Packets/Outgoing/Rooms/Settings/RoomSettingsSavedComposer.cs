namespace Plus.Communication.Packets.Outgoing.Rooms.Settings
{
    class RoomSettingsSavedComposer : ServerPacket
    {
        public RoomSettingsSavedComposer(int roomID)
            : base(ServerPacketHeader.RoomSettingsSavedMessageComposer)
        {
            WriteInteger(roomID);
        }
    }
}