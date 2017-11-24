using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class UserRemoveComposer : ServerPacket
    {
        public UserRemoveComposer(int Id)
            : base(ServerPacketHeader.UserRemoveMessageComposer)
        {
           base.WriteString(Id.ToString());
        }
    }
}
