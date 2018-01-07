using System;
using System.Data;
using System.Threading;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.GameClients;

using Plus.Communication.Packets.Outgoing.Rooms.Engine;


using Plus.Communication.Packets.Outgoing.Rooms.Furni;

using Plus.Database.Interfaces;
using Plus.HabboHotel.Cache.Type;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni
{
    class OpenGiftEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            int PresentId = Packet.PopInt();
            Item Present = Room.GetRoomItemHandler().GetItem(PresentId);
            if (Present == null)
                return;

            if (Present.UserID != Session.GetHabbo().Id)
                return;
            
            DataRow Data = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `base_id`,`extra_data` FROM `user_presents` WHERE `item_id` = @presentId LIMIT 1");
                dbClient.AddParameter("presentId", Present.Id);
                Data = dbClient.GetRow();
            }

            if (Data == null)
            {
                Session.SendNotification("Oops! Appears there was a bug with this gift.\nWe'll just get rid of it for you.");
                Room.GetRoomItemHandler().RemoveFurniture(null, Present.Id);

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + Present.Id + "' LIMIT 1");
                    dbClient.RunQuery("DELETE FROM `user_presents` WHERE `item_id` = '" + Present.Id + "' LIMIT 1");
                }

                Session.GetHabbo().GetInventoryComponent().RemoveItem(Present.Id);
                return;
            }
            
            if (!int.TryParse(Present.ExtraData.Split(Convert.ToChar(5))[2], out int PurchaserId))
            {
                Session.SendNotification("Oops! Appears there was a bug with this gift.\nWe'll just get rid of it for you.");
                Room.GetRoomItemHandler().RemoveFurniture(null, Present.Id);

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + Present.Id + "' LIMIT 1");
                    dbClient.RunQuery("DELETE FROM `user_presents` WHERE `item_id` = '" + Present.Id + "' LIMIT 1");
                }
                Session.GetHabbo().GetInventoryComponent().RemoveItem(Present.Id);

                return;
            }

            UserCache Purchaser = PlusEnvironment.GetGame().GetCacheManager().GenerateUser(PurchaserId);
            if (Purchaser == null)
            {
                Session.SendNotification("Oops! Appears there was a bug with this gift.\nWe'll just get rid of it for you.");
                Room.GetRoomItemHandler().RemoveFurniture(null, Present.Id);

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + Present.Id + "' LIMIT 1");
                    dbClient.RunQuery("DELETE FROM `user_presents` WHERE `item_id` = '" + Present.Id + "' LIMIT 1");
                }

                Session.GetHabbo().GetInventoryComponent().RemoveItem(Present.Id);
                return;
            }
            
            if (!PlusEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(Data["base_id"]), out ItemData BaseItem))
            {
                Session.SendNotification("Oops, it appears that the item within the gift is no longer in the hotel!");
                Room.GetRoomItemHandler().RemoveFurniture(null, Present.Id);

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + Present.Id + "' LIMIT 1");
                    dbClient.RunQuery("DELETE FROM `user_presents` WHERE `item_id` = '" + Present.Id + "' LIMIT 1");
                }

                Session.GetHabbo().GetInventoryComponent().RemoveItem(Present.Id);
                return;
            }


            Present.MagicRemove = true;
            Room.SendPacket(new ObjectUpdateComposer(Present, Convert.ToInt32(Session.GetHabbo().Id)));

            Thread thread = new Thread(() => FinishOpenGift(Session, BaseItem, Present, Room, Data));
            thread.Start();


        }

        private void FinishOpenGift(GameClient Session, ItemData BaseItem, Item Present, Room Room, DataRow Row)
        {
            try
            {
                if (BaseItem == null || Present == null || Room == null || Row == null)
                    return;


                Thread.Sleep(1500);

                bool ItemIsInRoom = true;

                Room.GetRoomItemHandler().RemoveFurniture(Session, Present.Id);

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `items` SET `base_item` = @BaseItem, `extra_data` = @edata WHERE `id` = @itemId LIMIT 1");
                    dbClient.AddParameter("itemId", Present.Id);
                    dbClient.AddParameter("BaseItem", Row["base_id"]);
                    dbClient.AddParameter("edata", Row["extra_data"]);
                    dbClient.RunQuery();

                    dbClient.RunQuery("DELETE FROM `user_presents` WHERE `item_id` = " + Present.Id + " LIMIT 1");
                }

                Present.BaseItem = Convert.ToInt32(Row["base_id"]);
                Present.ResetBaseItem();
                Present.ExtraData = (!string.IsNullOrEmpty(Convert.ToString(Row["extra_data"])) ? Convert.ToString(Row["extra_data"]) : "");

                if (Present.Data.Type == 's')
                {
                    if (!Room.GetRoomItemHandler().SetFloorItem(Session, Present, Present.GetX, Present.GetY, Present.Rotation, true, false, true))
                    {
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = @itemId LIMIT 1");
                            dbClient.AddParameter("itemId", Present.Id);
                            dbClient.RunQuery();
                        }

                        ItemIsInRoom = false;
                    }
                }
                else
                {
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = @itemId LIMIT 1");
                        dbClient.AddParameter("itemId", Present.Id);
                        dbClient.RunQuery();
                    }

                    ItemIsInRoom = false;
                }

                Session.SendPacket(new OpenGiftComposer(Present.Data, Present.ExtraData, Present, ItemIsInRoom));

                Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
            }
            catch
            {
            }
        }
    }
}