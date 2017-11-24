using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms.Trading;
using Plus.Communication.Packets.Outgoing.Inventory.Trading;

namespace Plus.Communication.Packets.Incoming.Inventory.Trading
{
    class TradingRemoveItemEvent : IPacketEvent
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

            TradeUser User = Trade.Users[0];
            if (User.RoomUser != RoomUser)
                User = Trade.Users[1];

            if (!User.OfferedItems.ContainsKey(Item.Id))
                return;

            Trade.RemoveAccepted();
            User.OfferedItems.Remove(Item.Id);
            Trade.SendPacket(new TradingUpdateComposer(Trade));
        }
    }
}