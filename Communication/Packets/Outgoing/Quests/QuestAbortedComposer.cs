namespace Plus.Communication.Packets.Outgoing.Quests
{
    class QuestAbortedComposer : ServerPacket
    {
        public QuestAbortedComposer()
            : base(ServerPacketHeader.QuestAbortedMessageComposer)
        {
            base.WriteBoolean(false);
        }
    }
}
