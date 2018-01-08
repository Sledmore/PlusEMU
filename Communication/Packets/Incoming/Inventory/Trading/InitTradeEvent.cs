using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Rooms.Trading;
using Plus.Communication.Packets.Outgoing.Inventory.Trading;

using Plus.Database.Interfaces;

namespace Plus.Communication.Packets.Incoming.Inventory.Trading
{
    class InitTradeEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int UserId = Packet.PopInt();

            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            RoomUser RoomUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (RoomUser == null)
                return;

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByVirtualId(UserId);
            if (TargetUser == null)
                return;

            if (Session.GetHabbo().TradingLockExpiry > 0)
            {
                if (Session.GetHabbo().TradingLockExpiry > PlusEnvironment.GetUnixTimestamp())
                {
                    Session.SendNotification("You're currently banned from trading.");
                    return;
                }
                else
                {
                    Session.GetHabbo().TradingLockExpiry = 0;
                    Session.SendNotification("Your trading ban has now expired.");

                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `user_info` SET `trading_locked` = '0' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    }
                }
            }

            if (!Session.GetHabbo().GetPermissions().HasRight("room_trade_override"))
            {
                if (Room.TradeSettings == 0)
                {
                    Session.SendPacket(new TradingErrorComposer(6, TargetUser.GetUsername()));
                    return;
                }
                else if (Room.TradeSettings == 1 && Room.OwnerId != Session.GetHabbo().Id)
                {
                    Session.SendPacket(new TradingErrorComposer(6, TargetUser.GetUsername()));
                    return;
                }
            }

            if (RoomUser.IsTrading && RoomUser.TradePartner != TargetUser.UserId)
            {
                Session.SendPacket(new TradingErrorComposer(7, TargetUser.GetUsername()));
                return;
            }
            else if (TargetUser.IsTrading && TargetUser.TradePartner != RoomUser.UserId)
            {
                Session.SendPacket(new TradingErrorComposer(8, TargetUser.GetUsername()));
                return;
            }
            else if (!TargetUser.GetClient().GetHabbo().AllowTradingRequests)
            {
                Session.SendPacket(new TradingErrorComposer(4, TargetUser.GetUsername()));
                return;
            }
            else if (TargetUser.GetClient().GetHabbo().TradingLockExpiry > 0)
            {
                Session.SendPacket(new TradingErrorComposer(4, TargetUser.GetUsername()));
                return;
            }

            Trade Trade = null;
            if (!Room.GetTrading().StartTrade(RoomUser, TargetUser, out Trade))
            {
                Session.SendNotification("An error occured trying to start this trade");
                return;
            }

            if (TargetUser.HasStatus("trd"))
                TargetUser.RemoveStatus("trd");
            if (RoomUser.HasStatus("trd"))
                RoomUser.RemoveStatus("trd");

            TargetUser.SetStatus("trd");
            TargetUser.UpdateNeeded = true;
            RoomUser.SetStatus("trd");
            RoomUser.UpdateNeeded = true;

            Trade.SendPacket(new TradingStartComposer(RoomUser.UserId, TargetUser.UserId));
        }
    }
}