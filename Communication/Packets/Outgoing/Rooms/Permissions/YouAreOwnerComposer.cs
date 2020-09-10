namespace Plus.Communication.Packets.Outgoing.Rooms.Permissions
{
    class YouAreOwnerComposer : MessageComposer
    {
        public YouAreOwnerComposer()
            : base(ServerPacketHeader.YouAreOwnerMessageComposer)
        {
        }

        public override void Compose(ServerPacket packet)
        {
            
        }
    }
}
