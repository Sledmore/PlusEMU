using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Handshake
{
    class SetUniqueIdComposer : ServerPacket
    {
        public SetUniqueIdComposer(string Id)
            : base(ServerPacketHeader.SetUniqueIdMessageComposer)
        {
           base.WriteString(Id);
        }
    }
}
