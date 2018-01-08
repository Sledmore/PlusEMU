using System;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class RoomEventComposer : ServerPacket
    {
        public RoomEventComposer(RoomData Data, RoomPromotion Promotion)
            : base(ServerPacketHeader.RoomEventMessageComposer)
        {
            WriteInteger(Promotion == null ? -1 : Convert.ToInt32(Data.Id));
            WriteInteger(Promotion == null ? -1 : Data.OwnerId);
           WriteString(Promotion == null ? "" : Data.OwnerName);
            WriteInteger(Promotion == null ? 0 : 1);
            WriteInteger(0);
           WriteString(Promotion == null ? "" : Promotion.Name);
           WriteString(Promotion == null ? "" : Promotion.Description);
            WriteInteger(0);
            WriteInteger(0);
            WriteInteger(0);//Unknown, came in build RELEASE63-201411181343-400753188
        }
    }
}
