using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class PurchaseErrorComposer : ServerPacket
    {
        public PurchaseErrorComposer(int ErrorCode)
            : base(ServerPacketHeader.PurchaseErrorMessageComposer)
        {
            base.WriteInteger(ErrorCode);
        }
    }
}
