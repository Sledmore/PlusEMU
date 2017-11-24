using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Rooms.Notifications
{
    class RoomErrorNotifComposer : ServerPacket
    {
        public RoomErrorNotifComposer(int Error)
            : base(ServerPacketHeader.RoomErrorNotifMessageComposer)
        {
            base.WriteInteger(Error);
        }
    }
}
