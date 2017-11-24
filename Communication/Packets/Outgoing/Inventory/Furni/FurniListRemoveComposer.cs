using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Inventory.Furni
{
    class FurniListRemoveComposer : ServerPacket
    {
        public FurniListRemoveComposer(int Id)
            : base(ServerPacketHeader.FurniListRemoveMessageComposer)
        {
            base.WriteInteger(Id);
        }
    }
}
