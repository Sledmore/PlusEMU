namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.LoveLocks
{
    class LoveLockDialogueSetLockedMessageComposer : ServerPacket
    {
        public LoveLockDialogueSetLockedMessageComposer(int ItemId)
            : base(ServerPacketHeader.LoveLockDialogueSetLockedMessageComposer)
        {
            base.WriteInteger(ItemId);
        }
    }
}
