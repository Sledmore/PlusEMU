using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.Rooms.Settings
{
    class FlatControllerAddedComposer : ServerPacket
    {
        public FlatControllerAddedComposer(int RoomId, int UserId, string Username)
            : base(ServerPacketHeader.FlatControllerAddedMessageComposer)
        {
            base.WriteInteger(RoomId);
            base.WriteInteger(UserId);
           base.WriteString(Username);
        }
    }
}
