using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Rooms.Polls.Questions
{
    class QuestionParserComposer : ServerPacket
    {
        public QuestionParserComposer()
            : base(ServerPacketHeader.QuestionParserMessageComposer)
        {
            base.WriteString("MATCHING_POLL");
            base.WriteInteger(2686);//??
            base.WriteInteger(10016);//???
            base.WriteInteger(60);//Duration
            base.WriteInteger(10016);
            base.WriteInteger(9);
            base.WriteInteger(6);
            base.WriteString("MAFIA WARS: WEAPONS VOTE");
            base.WriteInteger(0);
            base.WriteInteger(6);
            base.WriteInteger(0);
            base.WriteInteger(0);
        }
    }
}
