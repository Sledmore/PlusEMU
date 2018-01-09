namespace Plus.Communication.Packets.Outgoing.Rooms.Polls.Questions
{
    class QuestionParserComposer : ServerPacket
    {
        public QuestionParserComposer()
            : base(ServerPacketHeader.QuestionParserMessageComposer)
        {
            WriteString("MATCHING_POLL");
            WriteInteger(2686);//??
            WriteInteger(10016);//???
            WriteInteger(60);//Duration
            WriteInteger(10016);
            WriteInteger(9);
            WriteInteger(6);
            WriteString("MAFIA WARS: WEAPONS VOTE");
            WriteInteger(0);
            WriteInteger(6);
            WriteInteger(0);
            WriteInteger(0);
        }
    }
}
