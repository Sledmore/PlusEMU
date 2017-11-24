using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class ModeratorSupportTicketResponseComposer : ServerPacket
    {
        public ModeratorSupportTicketResponseComposer(int result)
            : base(ServerPacketHeader.ModeratorSupportTicketResponseMessageComposer)
        {
            base.WriteInteger(result);
            base.WriteString("");
        }
    }
}