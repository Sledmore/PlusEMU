using System.Linq;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms.Trading;
using Plus.Communication.Packets.Outgoing.Inventory.Trading;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Inventory.Trading
{
    class TradingOfferItemEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || !session.GetHabbo().InRoom)
                return;

            Room room = session.GetHabbo().CurrentRoom;
            if (room == null)
                return;

            RoomUser roomUser = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
            if (roomUser == null)
                return;

            int itemId = packet.PopInt();

            if (!roomUser.IsTrading)
            {
                session.SendPacket(new TradingClosedComposer(session.GetHabbo().Id));
                return;
            }

            if (!room.GetTrading().TryGetTrade(roomUser.TradeId, out Trade trade))
            {
                session.SendPacket(new TradingClosedComposer(session.GetHabbo().Id));
                return;
            }

            Item item = session.GetHabbo().GetInventoryComponent().GetItem(itemId);
            if (item == null)
                return;

            if (!trade.CanChange)
                return;

            TradeUser tradeUser = trade.Users[0];

            if (tradeUser.RoomUser != roomUser)
                tradeUser = trade.Users[1];

            if (tradeUser.OfferedItems.ContainsKey(item.Id))
                return;

            trade.RemoveAccepted();

            if (tradeUser.OfferedItems.Count <= 499)
            {
                int totalLtDs = tradeUser.OfferedItems.Count(x => x.Value.LimitedNo > 0);

                if (totalLtDs < 9)
                    tradeUser.OfferedItems.Add(item.Id, item);
            }

            trade.SendPacket(new TradingUpdateComposer(trade));
        }
    }
}