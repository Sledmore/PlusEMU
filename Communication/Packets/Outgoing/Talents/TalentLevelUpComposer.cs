using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.Talents
{
    class TalentLevelUpComposer : ServerPacket
    {
        public TalentLevelUpComposer()
            : base(ServerPacketHeader.TalentLevelUpMessageComposer)
        {

        }
    }
}
