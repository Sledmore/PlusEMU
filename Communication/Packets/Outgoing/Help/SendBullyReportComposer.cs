using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.Help
{
    class SendBullyReportComposer : ServerPacket
    {
        public SendBullyReportComposer()
            : base(ServerPacketHeader.SendBullyReportMessageComposer)
        {
            base.WriteInteger(0);//0-3, sends 0 on Habbo for this purpose.
        }
    }
}
