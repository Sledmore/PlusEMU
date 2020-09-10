namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.LoveLocks
{
    class LoveLockDialogueCloseMessageComposer : MessageComposer
    {
        public int ItemId { get; }

        public LoveLockDialogueCloseMessageComposer(int ItemId)
            : base(ServerPacketHeader.LoveLockDialogueCloseMessageComposer)
        {
            this.ItemId = ItemId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(ItemId);
        }
    }
}
