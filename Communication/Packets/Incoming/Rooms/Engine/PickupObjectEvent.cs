using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Quests;
using Plus.HabboHotel.GameClients;

using Plus.Database.Interfaces;

namespace Plus.Communication.Packets.Incoming.Rooms.Engine
{
    class PickupObjectEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {

            if (!session.GetHabbo().InRoom)
                return;

            Room Room = session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            int unknown = packet.PopInt();
            int itemId = packet.PopInt();

            Item item = Room.GetRoomItemHandler().GetItem(itemId);
            if (item == null)
                return;

            if (item.GetBaseItem().InteractionType == InteractionType.POSTIT)
                return;

            bool ItemRights = false;
            if (item.UserID == session.GetHabbo().Id || Room.CheckRights(session, false))
                ItemRights = true;
            else if (Room.Group != null && Room.CheckRights(session, false, true))//Room has a group, this user has group rights.
                ItemRights = true;
            else if (session.GetHabbo().GetPermissions().HasRight("room_item_take"))
                ItemRights = true;

            if (ItemRights == true)
            {
                if (item.GetBaseItem().InteractionType == InteractionType.TENT || item.GetBaseItem().InteractionType == InteractionType.TENT_SMALL)
                    Room.RemoveTent(item.Id);

                if (item.GetBaseItem().InteractionType == InteractionType.MOODLIGHT)
                {
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("DELETE FROM `room_items_moodlight` WHERE `item_id` = '" + item.Id + "' LIMIT 1");
                    }
                }
                else if (item.GetBaseItem().InteractionType == InteractionType.TONER)
                {
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("DELETE FROM `room_items_toner` WHERE `id` = '" + item.Id + "' LIMIT 1");
                    }
                }


                if (item.UserID == session.GetHabbo().Id)
                {
                    Room.GetRoomItemHandler().RemoveFurniture(session, item.Id);
                    session.GetHabbo().GetInventoryComponent().AddNewItem(item.Id, item.BaseItem, item.ExtraData, item.GroupId, true, true, item.LimitedNo, item.LimitedTot);
                    session.GetHabbo().GetInventoryComponent().UpdateItems(false);
                }
                else if (session.GetHabbo().GetPermissions().HasRight("room_item_take"))//Staff are taking this item
                {
                    Room.GetRoomItemHandler().RemoveFurniture(session, item.Id);
                    session.GetHabbo().GetInventoryComponent().AddNewItem(item.Id, item.BaseItem, item.ExtraData, item.GroupId, true, true, item.LimitedNo, item.LimitedTot);
                    session.GetHabbo().GetInventoryComponent().UpdateItems(false);

                }
                else//Item is being ejected.
                {
                    GameClient targetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(item.UserID);
                    if (targetClient != null && targetClient.GetHabbo() != null)//Again, do we have an active client?
                    {
                        Room.GetRoomItemHandler().RemoveFurniture(targetClient, item.Id);
                        targetClient.GetHabbo().GetInventoryComponent().AddNewItem(item.Id, item.BaseItem, item.ExtraData, item.GroupId, true, true, item.LimitedNo, item.LimitedTot);
                        targetClient.GetHabbo().GetInventoryComponent().UpdateItems(false);
                    }
                    else//No, query time.
                    {
                        Room.GetRoomItemHandler().RemoveFurniture(null, item.Id);
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = '" + item.Id + "' LIMIT 1");
                        }
                    }
                }

                PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.FurniPick);
            }
        }
    }
}