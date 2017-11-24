using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Handshake
{
    class AvailabilityStatusComposer : ServerPacket
    {
        public AvailabilityStatusComposer()
            : base(ServerPacketHeader.AvailabilityStatusMessageComposer)
        {
            base.WriteBoolean(true);
            base.WriteBoolean(false);
            base.WriteBoolean(true);
        }
    }
}
