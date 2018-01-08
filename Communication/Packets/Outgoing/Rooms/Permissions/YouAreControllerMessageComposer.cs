namespace Plus.Communication.Packets.Outgoing.Rooms.Permissions
{
    class YouAreControllerComposer : ServerPacket
    {
        public YouAreControllerComposer(int Setting)
            : base(ServerPacketHeader.YouAreControllerMessageComposer)
        {
            base.WriteInteger(Setting);
        }
    }
}
