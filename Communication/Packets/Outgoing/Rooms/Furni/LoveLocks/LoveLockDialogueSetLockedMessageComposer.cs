namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.LoveLocks
{
    class LoveLockDialogueSetLockedMessageComposer : MessageComposer
    {
        public int ItemId { get; }

        public LoveLockDialogueSetLockedMessageComposer(int ItemId)
            : base(ServerPacketHeader.LoveLockDialogueSetLockedMessageComposer)
        {
            this.ItemId = ItemId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(ItemId);
        }
    }
}
