using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Items;
using Plus.Communication.Packets.Outgoing.Inventory.Trading;
using Plus.Communication.Packets.Outgoing;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.Communication.Packets.Outgoing.Inventory.Furni;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.HabboHotel.Rooms.Trading
{
    public sealed class Trade
    {
        public int Id { get; set; }
        public TradeUser[] Users { get; set; }
        public bool CanChange { get; set; }

        private Room Instance = null;

        public Trade(int id, RoomUser playerOne, RoomUser playerTwo, Room room)
        {
            Id = id;
            CanChange = true;
            Instance = room;
            Users = new TradeUser[2];
            Users[0] = new TradeUser(playerOne);
            Users[1] = new TradeUser(playerTwo);

            playerOne.IsTrading = true;
            playerOne.TradeId = Id;
            playerOne.TradePartner = playerTwo.UserId;
            playerTwo.IsTrading = true;
            playerTwo.TradeId = Id;
            playerTwo.TradePartner = playerOne.UserId;
        }

        public void SendPacket(MessageComposer packet)
        {
            foreach (TradeUser user in Users)
            {
                if (user == null || user.RoomUser == null || user.RoomUser.GetClient() == null)
                    continue;

                user.RoomUser.GetClient().SendPacket(packet);
            }
        }

        public void RemoveAccepted()
        {
            foreach (TradeUser User in Users)
            {
                if (User == null)
                    continue;

                User.HasAccepted = false;
            }
        }

        public bool AllAccepted
        {
            get
            {
                foreach (TradeUser User in Users)
                {
                    if (User == null)
                        continue;

                    if (!User.HasAccepted)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public void EndTrade(int UserId)
        {
            foreach (TradeUser TradeUser in Users)
            {
                if (TradeUser == null || TradeUser.RoomUser == null)
                    continue;

                RemoveTrade(TradeUser.RoomUser.UserId);
            }

            SendPacket(new TradingClosedComposer(UserId));
            Instance.GetTrading().RemoveTrade(Id);
        }

        public void Finish()
        {
            foreach (TradeUser TradeUser in Users)
            {
                if (TradeUser == null)
                    continue;

                RemoveTrade(TradeUser.RoomUser.UserId);
            }

            ProcessItems();
            SendPacket(new TradingFinishComposer());

            Instance.GetTrading().RemoveTrade(Id);
        }

        public void RemoveTrade(int UserId)
        {
            TradeUser TradeUser = Users[0];

            if (TradeUser.RoomUser.UserId != UserId)
            {
                TradeUser = Users[1];
            }

            TradeUser.RoomUser.RemoveStatus("trd");
            TradeUser.RoomUser.UpdateNeeded = true;
            TradeUser.RoomUser.IsTrading = false;
            TradeUser.RoomUser.TradeId = 0;
            TradeUser.RoomUser.TradePartner = 0;
        }

        public void ProcessItems()
        {
            List<Item> UserOne = Users[0].OfferedItems.Values.ToList();
            List<Item> UserTwo = Users[1].OfferedItems.Values.ToList();

            RoomUser RoomUserOne = Users[0].RoomUser;
            RoomUser RoomUserTwo = Users[1].RoomUser;

            string logUserOne = "";
            string logUserTwo = "";

            if (RoomUserOne == null || RoomUserOne.GetClient() == null || RoomUserOne.GetClient().GetHabbo() == null || RoomUserOne.GetClient().GetHabbo().GetInventoryComponent() == null)
                return;

            if (RoomUserTwo == null || RoomUserTwo.GetClient() == null || RoomUserTwo.GetClient().GetHabbo() == null || RoomUserTwo.GetClient().GetHabbo().GetInventoryComponent() == null)
                return;

            foreach (Item Item in UserOne)
            {
                Item I = RoomUserOne.GetClient().GetHabbo().GetInventoryComponent().GetItem(Item.Id);

                if (I == null)
                {
                    SendPacket(new BroadcastMessageAlertComposer("Error! Trading Failed!"));
                    return;
                }
            }

            foreach (Item Item in UserTwo)
            {
                Item I = RoomUserTwo.GetClient().GetHabbo().GetInventoryComponent().GetItem(Item.Id);

                if (I == null)
                {
                    SendPacket(new BroadcastMessageAlertComposer("Error! Trading Failed!"));
                    return;
                }
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                foreach (Item Item in UserOne)
                {
                    logUserOne += Item.Id + ";";
                    RoomUserOne.GetClient().GetHabbo().GetInventoryComponent().RemoveItem(Item.Id);
                    if (Item.Data.InteractionType == InteractionType.EXCHANGE && PlusEnvironment.GetSettingsManager().TryGetValue("trading.auto_exchange_redeemables") == "1")
                    {
                        RoomUserTwo.GetClient().GetHabbo().Credits += Item.Data.BehaviourData;
                        RoomUserTwo.GetClient().SendPacket(new CreditBalanceComposer(RoomUserTwo.GetClient().GetHabbo().Credits));

                        dbClient.SetQuery("DELETE FROM `items` WHERE `id` = @id LIMIT 1");
                        dbClient.AddParameter("id", Item.Id);
                        dbClient.RunQuery();
                    }
                    else
                    {
                        if (RoomUserTwo.GetClient().GetHabbo().GetInventoryComponent().TryAddItem(Item))
                        {
                            RoomUserTwo.GetClient().SendPacket(new FurniListAddComposer(Item));
                            RoomUserTwo.GetClient().SendPacket(new FurniListNotificationComposer(Item.Id, 1));

                            dbClient.SetQuery("UPDATE `items` SET `user_id` = @user WHERE id=@id LIMIT 1");
                            dbClient.AddParameter("user", RoomUserTwo.UserId);
                            dbClient.AddParameter("id", Item.Id);
                            dbClient.RunQuery();
                        }
                    }
                }

                foreach (Item Item in UserTwo)
                {
                    logUserTwo += Item.Id + ";";
                    RoomUserTwo.GetClient().GetHabbo().GetInventoryComponent().RemoveItem(Item.Id);
                    if (Item.Data.InteractionType == InteractionType.EXCHANGE && PlusEnvironment.GetSettingsManager().TryGetValue("trading.auto_exchange_redeemables") == "1")
                    {
                        RoomUserOne.GetClient().GetHabbo().Credits += Item.Data.BehaviourData;
                        RoomUserOne.GetClient().SendPacket(new CreditBalanceComposer(RoomUserOne.GetClient().GetHabbo().Credits));

                        dbClient.SetQuery("DELETE FROM `items` WHERE `id` = @id LIMIT 1");
                        dbClient.AddParameter("id", Item.Id);
                        dbClient.RunQuery();
                    }
                    else
                    {
                        if (RoomUserOne.GetClient().GetHabbo().GetInventoryComponent().TryAddItem(Item))
                        {
                            RoomUserOne.GetClient().SendPacket(new FurniListAddComposer(Item));
                            RoomUserOne.GetClient().SendPacket(new FurniListNotificationComposer(Item.Id, 1));

                            dbClient.SetQuery("UPDATE `items` SET `user_id` = @user WHERE id=@id LIMIT 1");
                            dbClient.AddParameter("user", RoomUserOne.UserId);
                            dbClient.AddParameter("id", Item.Id);
                            dbClient.RunQuery();
                        }
                    }
                }

                dbClient.SetQuery("INSERT INTO `logs_client_trade` VALUES(null, @1id, @2id, @1items, @2items, UNIX_TIMESTAMP())");
                dbClient.AddParameter("1id", RoomUserOne.UserId);
                dbClient.AddParameter("2id", RoomUserTwo.UserId);
                dbClient.AddParameter("1items", logUserOne);
                dbClient.AddParameter("2items", logUserTwo);
                dbClient.RunQuery();
            }
        }
    }
}