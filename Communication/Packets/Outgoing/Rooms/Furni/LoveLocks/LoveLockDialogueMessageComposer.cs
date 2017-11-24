namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.LoveLocks
{
    class LoveLockDialogueMessageComposer : ServerPacket
    {
        public LoveLockDialogueMessageComposer(int ItemId)
            : base(ServerPacketHeader.LoveLockDialogueMessageComposer)
        {
            base.WriteInteger(ItemId);
            base.WriteBoolean(true);
        }
    }
}
