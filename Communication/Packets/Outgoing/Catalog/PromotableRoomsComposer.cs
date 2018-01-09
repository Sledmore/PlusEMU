using System.Collections.Generic;
using Plus.HabboHotel.Rooms;
namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class PromotableRoomsComposer : ServerPacket
    {
        public PromotableRoomsComposer(ICollection<RoomData> Rooms)
            : base(ServerPacketHeader.PromotableRoomsMessageComposer)
        {
            WriteBoolean(true);
            WriteInteger(Rooms.Count);//Count

            foreach (RoomData Data in Rooms)
            {
                WriteInteger(Data.Id);
               WriteString(Data.Name);
                WriteBoolean(false);
            }
        }
    }
}