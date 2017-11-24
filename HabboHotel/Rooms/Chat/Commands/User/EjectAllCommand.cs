using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class EjectAllCommand :IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_ejectall"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Removes all of the items from the room."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Session.GetHabbo().Id == Room.OwnerId)
            {
                //Let us check anyway.
                if (!Room.CheckRights(Session, true))
                    return;

                foreach (Item Item in Room.GetRoomItemHandler().GetWallAndFloor.ToList())
                {
                    if (Item == null || Item.UserID == Session.GetHabbo().Id)
                        continue;

                    GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(Item.UserID);
                    if (TargetClient != null && TargetClient.GetHabbo() != null)
                    {
                        Room.GetRoomItemHandler().RemoveFurniture(TargetClient, Item.Id);
                        TargetClient.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, Item.BaseItem, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                        TargetClient.GetHabbo().GetInventoryComponent().UpdateItems(false);
                    }
                    else
                    {
                        Room.GetRoomItemHandler().RemoveFurniture(null, Item.Id);
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                        }
                    }
                }
            }
            else
            {
                foreach (Item Item in Room.GetRoomItemHandler().GetWallAndFloor.ToList())
                {
                    if (Item == null || Item.UserID != Session.GetHabbo().Id)
                        continue;

                    GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(Item.UserID);
                    if (TargetClient != null && TargetClient.GetHabbo() != null)
                    {
                        Room.GetRoomItemHandler().RemoveFurniture(TargetClient, Item.Id);
                        TargetClient.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, Item.BaseItem, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                        TargetClient.GetHabbo().GetInventoryComponent().UpdateItems(false);
                    }
                    else
                    {
                        Room.GetRoomItemHandler().RemoveFurniture(null, Item.Id);
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                        }
                    }
                }
            }
        }
    }
}
