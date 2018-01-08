using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms.Trading;
using Plus.Communication.Packets.Outgoing.Inventory.Trading;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Inventory.Trading
{
    class TradingRemoveItemEvent : IPacketEvent
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

            TradeUser user = trade.Users[0];
            if (user.RoomUser != roomUser)
                user = trade.Users[1];

            if (!user.OfferedItems.ContainsKey(item.Id))
                return;

            trade.RemoveAccepted();
            user.OfferedItems.Remove(item.Id);
            trade.SendPacket(new TradingUpdateComposer(trade));
        }
    }
}