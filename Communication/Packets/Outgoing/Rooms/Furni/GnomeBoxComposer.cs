namespace Plus.Communication.Packets.Outgoing.Rooms.Furni
{
    class GnomeBoxComposer : MessageComposer
    {
        public int ItemId { get; }

        public GnomeBoxComposer(int ItemId)
            : base(ServerPacketHeader.GnomeBoxMessageComposer)
        {
            this.ItemId = ItemId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(ItemId);
        }
    }
}
