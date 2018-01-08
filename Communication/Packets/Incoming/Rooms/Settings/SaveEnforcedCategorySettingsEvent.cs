using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Navigator;

namespace Plus.Communication.Packets.Incoming.Rooms.Settings
{
    class SaveEnforcedCategorySettingsEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(packet.PopInt(), out Room room))
                return;

            if (!room.CheckRights(session, true))
                return;

            int categoryId = packet.PopInt();
            int tradeSettings = packet.PopInt();

            if (tradeSettings < 0 || tradeSettings > 2)
                tradeSettings = 0;

            if (!PlusEnvironment.GetGame().GetNavigator().TryGetSearchResultList(categoryId, out SearchResultList searchResultList))
            {
                categoryId = 36;
            }

            if (searchResultList.CategoryType != NavigatorCategoryType.Category || searchResultList.RequiredRank > session.GetHabbo().Rank)
            {
                categoryId = 36;
            }
        }
    }
}
