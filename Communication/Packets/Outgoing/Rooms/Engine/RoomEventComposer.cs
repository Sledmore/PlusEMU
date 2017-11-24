using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class RoomEventComposer : ServerPacket
    {
        public RoomEventComposer(RoomData Data, RoomPromotion Promotion)
            : base(ServerPacketHeader.RoomEventMessageComposer)
        {
            base.WriteInteger(Promotion == null ? -1 : Convert.ToInt32(Data.Id));
            base.WriteInteger(Promotion == null ? -1 : Data.OwnerId);
           base.WriteString(Promotion == null ? "" : Data.OwnerName);
            base.WriteInteger(Promotion == null ? 0 : 1);
            base.WriteInteger(0);
           base.WriteString(Promotion == null ? "" : Promotion.Name);
           base.WriteString(Promotion == null ? "" : Promotion.Description);
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(0);//Unknown, came in build RELEASE63-201411181343-400753188
        }
    }
}
