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
    class TradingOfferItemsEvent : IPacketEvent
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

            int Amount = Packet.PopInt();
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

            List<Item> AllItems = Session.GetHabbo().GetInventoryComponent().GetItems.Where(x => x.Data.Id == Item.Data.Id).Take(Amount).ToList();
            foreach (Item I in AllItems)
            {
                if (TradeUser.OfferedItems.ContainsKey(I.Id))
                    return;

                Trade.RemoveAccepted();

                TradeUser.OfferedItems.Add(I.Id, I);
            }

            Trade.SendPacket(new TradingUpdateComposer(Trade));
        }
    }
}
