namespace Plus.Communication.Packets.Outgoing.Groups
{
    class RefreshFavouriteGroupComposer : MessageComposer
    {
        public int GroupId { get; }

        public RefreshFavouriteGroupComposer(int Id)
            : base(ServerPacketHeader.RefreshFavouriteGroupMessageComposer)
        {
            this.GroupId = Id;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Id);
        }
    }
}
