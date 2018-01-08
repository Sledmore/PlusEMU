using System.Linq;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms.Trading;
using Plus.Communication.Packets.Outgoing.Inventory.Trading;

namespace Plus.Communication.Packets.Incoming.Inventory.Trading
{
    class TradingOfferItemEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            RoomUser RoomUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (RoomUser == null)
                return;

            int ItemId = Packet.PopInt();

            Trade Trade = null;

            if (!RoomUser.IsTrading)
            {
                Session.SendPacket(new TradingClosedComposer(Session.GetHabbo().Id));
                return;
            }

            if (!Room.GetTrading().TryGetTrade(RoomUser.TradeId, out Trade))
            {
                Session.SendPacket(new TradingClosedComposer(Session.GetHabbo().Id));
                return;
            }

            Item Item = Session.GetHabbo().GetInventoryComponent().GetItem(ItemId);
            if (Item == null)
                return;

            if (!Trade.CanChange)
                return;

            TradeUser TradeUser = Trade.Users[0];

            if (TradeUser.RoomUser != RoomUser)
                TradeUser = Trade.Users[1];

            if (TradeUser.OfferedItems.ContainsKey(Item.Id))
                return;

            Trade.RemoveAccepted();

            if (TradeUser.OfferedItems.Count <= 499)
            {
                int TotalLTDs = TradeUser.OfferedItems.Where(x => x.Value.LimitedNo > 0).Count();

                if (TotalLTDs < 9)
                    TradeUser.OfferedItems.Add(Item.Id, Item);
            }

            Trade.SendPacket(new TradingUpdateComposer(Trade));
        }
    }
}