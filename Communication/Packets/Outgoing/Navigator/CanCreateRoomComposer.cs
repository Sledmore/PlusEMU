using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class CanCreateRoomComposer : ServerPacket
    {
        public CanCreateRoomComposer(bool Error, int MaxRoomsPerUser)
            : base(ServerPacketHeader.CanCreateRoomMessageComposer)
        {
            base.WriteInteger(Error ? 1 : 0);
            base.WriteInteger(MaxRoomsPerUser);
        }
    }
}
