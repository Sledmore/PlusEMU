using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Inventory.Furni
{
    class FurniListNotificationComposer : ServerPacket
    {
        public FurniListNotificationComposer(int Id, int Type)
            : base(ServerPacketHeader.FurniListNotificationMessageComposer)
        {
            base.WriteInteger(1);
            base.WriteInteger(Type);
            base.WriteInteger(1);
            base.WriteInteger(Id);
        }
    }
}