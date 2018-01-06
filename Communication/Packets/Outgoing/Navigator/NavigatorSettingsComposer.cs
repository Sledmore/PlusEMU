namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class NavigatorSettingsComposer : ServerPacket
    {
        public NavigatorSettingsComposer(int homeroom)
            : base(ServerPacketHeader.NavigatorSettingsMessageComposer)
        {
            WriteInteger(homeroom);
            WriteInteger(homeroom);
        }
    }
}
