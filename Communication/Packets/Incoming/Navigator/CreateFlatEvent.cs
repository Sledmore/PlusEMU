using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.HabboHotel.Navigator;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    class CreateFlatEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null)
                return;

            List<RoomData> rooms = RoomFactory.GetRoomsDataByOwnerSortByName(session.GetHabbo().Id);
            if (rooms.Count >= 500)
            {
                session.SendPacket(new CanCreateRoomComposer(true, 500));
                return;
            }

            string name = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
            string description = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
            string modelName = packet.PopString();

            int category = packet.PopInt();
            int maxVisitors = packet.PopInt();//10 = min, 25 = max.
            int tradeSettings = packet.PopInt();//2 = All can trade, 1 = owner only, 0 = no trading.

            if (name.Length < 3)
                return;

            if (name.Length > 25)
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetModel(modelName, out RoomModel model))
                return;

            if (!PlusEnvironment.GetGame().GetNavigator().TryGetSearchResultList(category, out SearchResultList searchResultList))
                category = 36;

            if (searchResultList.CategoryType != NavigatorCategoryType.Category || searchResultList.RequiredRank > session.GetHabbo().Rank)
                category = 36;

            if (maxVisitors < 10 || maxVisitors > 25)
                maxVisitors = 10;

            if (tradeSettings < 0 || tradeSettings > 2)
                tradeSettings = 0;

            RoomData newRoom = PlusEnvironment.GetGame().GetRoomManager().CreateRoom(session, name, description, category, maxVisitors, tradeSettings, model);
            if (newRoom != null)
            {
                session.SendPacket(new FlatCreatedComposer(newRoom.Id, name));
            }

            if (session.GetHabbo() != null && session.GetHabbo().GetMessenger() != null)
                session.GetHabbo().GetMessenger().OnStatusChanged(true);
        }
    }
}
