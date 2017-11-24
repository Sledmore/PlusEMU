using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Rooms.Session
{
    class CantConnectComposer : ServerPacket
    {
        public CantConnectComposer(int Error)
            : base(ServerPacketHeader.CantConnectMessageComposer)
        {
            base.WriteInteger(Error);
        }
    }
}
