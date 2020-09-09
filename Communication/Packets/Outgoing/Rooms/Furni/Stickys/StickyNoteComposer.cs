namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.Stickys
{
    class StickyNoteComposer : MessageComposer
    {
        public string ItemId { get; }
        public string ExtraData { get; }

        public StickyNoteComposer(string ItemId, string Extradata)
            : base(ServerPacketHeader.StickyNoteMessageComposer)
        {
            this.ItemId = ItemId;
            this.ExtraData = Extradata;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(ItemId);
            packet.WriteString(ExtraData);
        }
    }
}
