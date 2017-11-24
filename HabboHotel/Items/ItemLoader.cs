using System;
using System.Data;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.Database.Interfaces;


namespace Plus.HabboHotel.Items
{
    public static class ItemLoader
    {
        public static List<Item> GetItemsForRoom(int RoomId, Room Room)
        {
            DataTable Items = null;
            List<Item> I = new List<Item>();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `items`.*, COALESCE(`items_groups`.`group_id`, 0) AS `group_id` FROM `items` LEFT OUTER JOIN `items_groups` ON `items`.`id` = `items_groups`.`id` WHERE `items`.`room_id` = @rid;");
                dbClient.AddParameter("rid", RoomId);
                Items = dbClient.GetTable();

                if (Items != null)
                {
                    foreach (DataRow Row in Items.Rows)
                    {
                        ItemData Data = null;

                        if (PlusEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(Row["base_item"]), out Data))
                        {
                            I.Add(new Item(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["room_id"]), Convert.ToInt32(Row["base_item"]), Convert.ToString(Row["extra_data"]),
                                Convert.ToInt32(Row["x"]), Convert.ToInt32(Row["y"]), Convert.ToDouble(Row["z"]), Convert.ToInt32(Row["rot"]), Convert.ToInt32(Row["user_id"]), 
                                Convert.ToInt32(Row["group_id"]), Convert.ToInt32(Row["limited_number"]), Convert.ToInt32(Row["limited_stack"]), Convert.ToString(Row["wall_pos"]), Room));
                        }
                        else
                        {
                            // Item data does not exist anymore.
                        }
                    }
                }
            }
            return I;
        }

        public static List<Item> GetItemsForUser(int UserId)
        {
            DataTable Items = null;
            List<Item> I = new List<Item>();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `items`.*, COALESCE(`items_groups`.`group_id`, 0) AS `group_id` FROM `items` LEFT OUTER JOIN `items_groups` ON `items`.`id` = `items_groups`.`id` WHERE `items`.`room_id` = 0 AND `items`.`user_id` = @uid;");
                dbClient.AddParameter("uid", UserId);
                Items = dbClient.GetTable();

                if (Items != null)
                {
                    foreach (DataRow Row in Items.Rows)
                    {
                        ItemData Data = null;

                        if (PlusEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(Row["base_item"]), out Data))
                        {
                            I.Add(new Item(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["room_id"]), Convert.ToInt32(Row["base_item"]), Convert.ToString(Row["extra_data"]),
                                Convert.ToInt32(Row["x"]), Convert.ToInt32(Row["y"]), Convert.ToDouble(Row["z"]), Convert.ToInt32(Row["rot"]), Convert.ToInt32(Row["user_id"]),
                                Convert.ToInt32(Row["group_id"]), Convert.ToInt32(Row["limited_number"]), Convert.ToInt32(Row["limited_stack"]), Convert.ToString(Row["wall_pos"])));
                        }
                        else
                        {
                            // Item data does not exist anymore.
                        }
                    }
                }
            }
            return I;
        }
    }
}
