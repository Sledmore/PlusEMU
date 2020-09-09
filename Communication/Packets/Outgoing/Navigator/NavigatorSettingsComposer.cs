namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class NavigatorSettingsComposer : MessageComposer
    {
        public int HomeRoomId { get; }

        public NavigatorSettingsComposer(int homeroom)
            : base(ServerPacketHeader.NavigatorSettingsMessageComposer)
        {
            this.HomeRoomId = homeroom;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(HomeRoomId);
            packet.WriteInteger(HomeRoomId);
        }
    }
}
