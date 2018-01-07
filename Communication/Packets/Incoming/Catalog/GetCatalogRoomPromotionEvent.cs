using System.Collections.Generic;

using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    class GetCatalogRoomPromotionEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            List<RoomData> rooms = RoomFactory.GetRoomsDataByOwnerSortByName(session.GetHabbo().Id);

            session.SendPacket(new GetCatalogRoomPromotionComposer(rooms));
        }
    }
}
