namespace Plus.Communication.Packets.Outgoing.Rooms.Permissions
{
    class YouAreControllerComposer : MessageComposer
    {
        public int Setting { get; }
        public YouAreControllerComposer(int Setting)
            : base(ServerPacketHeader.YouAreControllerMessageComposer)
        {
            this.Setting = Setting;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Setting);
        }
    }
}
