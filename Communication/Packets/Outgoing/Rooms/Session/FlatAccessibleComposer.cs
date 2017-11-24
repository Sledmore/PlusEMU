using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Rooms.Session
{
    class FlatAccessibleComposer : ServerPacket
    {
        public FlatAccessibleComposer(string Username)
            : base(ServerPacketHeader.FlatAccessibleMessageComposer)
        {
           base.WriteString(Username);
        }
    }
}
