using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class FlatCreatedComposer : ServerPacket
    {
        public FlatCreatedComposer(int roomID, string roomName)
            : base(ServerPacketHeader.FlatCreatedMessageComposer)
        {
            base.WriteInteger(roomID);
           base.WriteString(roomName);
        }
    }
}
