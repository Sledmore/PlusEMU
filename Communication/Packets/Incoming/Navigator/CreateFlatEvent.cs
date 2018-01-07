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

            string Name = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
            string Description = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
            string ModelName = packet.PopString();

            int Category = packet.PopInt();
            int MaxVisitors = packet.PopInt();//10 = min, 25 = max.
            int TradeSettings = packet.PopInt();//2 = All can trade, 1 = owner only, 0 = no trading.

            if (Name.Length < 3)
                return;

            if (Name.Length > 25)
                return;

            RoomModel model = null;
            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetModel(ModelName, out model))
                return;

            SearchResultList SearchResultList = null;
            if (!PlusEnvironment.GetGame().GetNavigator().TryGetSearchResultList(Category, out SearchResultList))
                Category = 36;

            if (SearchResultList.CategoryType != NavigatorCategoryType.Category || SearchResultList.RequiredRank > session.GetHabbo().Rank)
                Category = 36;

            if (MaxVisitors < 10 || MaxVisitors > 25)
                MaxVisitors = 10;

            if (TradeSettings < 0 || TradeSettings > 2)
                TradeSettings = 0;

            RoomData NewRoom = PlusEnvironment.GetGame().GetRoomManager().CreateRoom(session, Name, Description, Category, MaxVisitors, TradeSettings, model);
            if (NewRoom != null)
            {
                session.SendPacket(new FlatCreatedComposer(NewRoom.Id, Name));
            }

            if (session.GetHabbo() != null && session.GetHabbo().GetMessenger() != null)
                session.GetHabbo().GetMessenger().OnStatusChanged(true);
        }
    }
}
