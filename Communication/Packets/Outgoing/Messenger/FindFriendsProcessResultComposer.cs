using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class FindFriendsProcessResultComposer : ServerPacket
    {
        public FindFriendsProcessResultComposer(bool Found)
            : base(ServerPacketHeader.FindFriendsProcessResultMessageComposer)
        {
            base.WriteBoolean(Found);
        }
    }
}