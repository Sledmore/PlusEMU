using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Notifications
{
    class MOTDNotificationComposer : ServerPacket
    {
        public MOTDNotificationComposer(string Message)
            : base(ServerPacketHeader.MOTDNotificationMessageComposer)
        {
            base.WriteInteger(1);
           base.WriteString(Message);
        }
    }
}
