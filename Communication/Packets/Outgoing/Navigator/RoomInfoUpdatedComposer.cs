using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class RoomInfoUpdatedComposer : ServerPacket
    {
        public RoomInfoUpdatedComposer(int roomID)
            : base(ServerPacketHeader.RoomInfoUpdatedMessageComposer)
        {
            base.WriteInteger(roomID);
        }
    }
}
