using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms.Trading;
using Plus.Communication.Packets.Outgoing.Inventory.Trading;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Inventory.Trading
{
    class TradingOfferItemsEvent : IPacketEvent
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

            int amount = packet.PopInt();
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

            List<Item> allItems = session.GetHabbo().GetInventoryComponent().GetItems.Where(x => x.Data.Id == item.Data.Id).Take(amount).ToList();
            foreach (Item I in allItems)
            {
                if (tradeUser.OfferedItems.ContainsKey(I.Id))
                    return;

                trade.RemoveAccepted();

                tradeUser.OfferedItems.Add(I.Id, I);
            }

            trade.SendPacket(new TradingUpdateComposer(trade));
        }
    }
}
