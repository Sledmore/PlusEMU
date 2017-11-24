using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Groups
{
    class UnknownGroupComposer : ServerPacket
    {
        public UnknownGroupComposer(int GroupId, int HabboId)
            : base(ServerPacketHeader.UnknownGroupMessageComposer)
        {
            base.WriteInteger(GroupId);
            base.WriteInteger(HabboId);
        }
    }
}