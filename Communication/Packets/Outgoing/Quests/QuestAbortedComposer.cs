namespace Plus.Communication.Packets.Outgoing.Quests
{
    class QuestAbortedComposer : MessageComposer
    {
        public QuestAbortedComposer()
            : base(ServerPacketHeader.QuestAbortedMessageComposer)
        {
            
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteBoolean(false);
        }
    }
}
