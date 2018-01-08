namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.LoveLocks
{
    class LoveLockDialogueMessageComposer : ServerPacket
    {
        public LoveLockDialogueMessageComposer(int ItemId)
            : base(ServerPacketHeader.LoveLockDialogueMessageComposer)
        {
            WriteInteger(ItemId);
            WriteBoolean(true);
        }
    }
}
