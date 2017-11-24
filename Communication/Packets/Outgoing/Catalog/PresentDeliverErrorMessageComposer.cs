using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class PresentDeliverErrorMessageComposer : ServerPacket
    {
        public PresentDeliverErrorMessageComposer(bool CreditError, bool DucketError)
            : base(ServerPacketHeader.PresentDeliverErrorMessageComposer)
        {
            base.WriteBoolean(CreditError);//Do we have enough credits?
            base.WriteBoolean(DucketError);//Do we have enough duckets?
        }
    }
}
