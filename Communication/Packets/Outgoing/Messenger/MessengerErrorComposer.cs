using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class MessengerErrorComposer : ServerPacket
    {
        public MessengerErrorComposer(int ErrorCode1, int ErrorCode2)
            : base(ServerPacketHeader.MessengerErrorMessageComposer)
        {
            base.WriteInteger(ErrorCode1);
            base.WriteInteger(ErrorCode2);
        }
    }
}
