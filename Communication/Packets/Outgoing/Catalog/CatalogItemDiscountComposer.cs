using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class CatalogItemDiscountComposer : ServerPacket
    {
        public CatalogItemDiscountComposer()
            : base(ServerPacketHeader.CatalogItemDiscountMessageComposer)
        {
            base.WriteInteger(100);//Most you can get.
            base.WriteInteger(6);
            base.WriteInteger(1);
            base.WriteInteger(1);
            base.WriteInteger(2);//Count
            {
                base.WriteInteger(40);
                base.WriteInteger(99);
            }
        }
    }
}