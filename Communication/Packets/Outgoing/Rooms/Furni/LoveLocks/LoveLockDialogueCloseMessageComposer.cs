namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.LoveLocks
{
    class LoveLockDialogueCloseMessageComposer : ServerPacket
    {
        public LoveLockDialogueCloseMessageComposer(int ItemId)
            : base(ServerPacketHeader.LoveLockDialogueCloseMessageComposer)
        {
            base.WriteInteger(ItemId);
        }
    }
}
