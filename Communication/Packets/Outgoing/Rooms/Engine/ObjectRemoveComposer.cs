using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class ObjectRemoveComposer : ServerPacket
    {
        public ObjectRemoveComposer(Item Item, int UserId)
            : base(ServerPacketHeader.ObjectRemoveMessageComposer)
        {
           base.WriteString(Item.Id.ToString());
            base.WriteBoolean(false);
            base.WriteInteger(UserId);
            base.WriteInteger(0);
        }
    }
}