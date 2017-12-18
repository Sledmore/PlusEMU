using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Catalog;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    class GetPromotableRoomsEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            List<RoomData> rooms = RoomFactory.GetRoomsDataByOwnerSortByName(session.GetHabbo().Id);

            rooms = rooms.Where(x => (x.Promotion == null || x.Promotion.TimestampExpires < Plus.Utilities.UnixTimestamp.GetNow())).ToList();

            session.SendPacket(new PromotableRoomsComposer(rooms));
        }
    }
}
