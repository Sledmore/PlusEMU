using System;
using System.Data;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.Database.Interfaces;


namespace Plus.HabboHotel.Items
{
    public static class ItemLoader
    {
        public static List<Item> GetItemsForRoom(int roomId, Room room)
        {
            List<Item> items = new List<Item>();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `items`.*, COALESCE(`items_groups`.`group_id`, 0) AS `group_id` FROM `items` LEFT OUTER JOIN `items_groups` ON `items`.`id` = `items_groups`.`id` WHERE `items`.`room_id` = @rid;");
                dbClient.AddParameter("rid", roomId);
                DataTable data = dbClient.GetTable();

                if (data != null)
                {
                    foreach (DataRow row in data.Rows)
                    {
                        if (PlusEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(row["base_item"]), out ItemData Data))
                        {
                            items.Add(new Item(Convert.ToInt32(row["id"]), Convert.ToInt32(row["room_id"]), Convert.ToInt32(row["base_item"]), Convert.ToString(row["extra_data"]),
                                Convert.ToInt32(row["x"]), Convert.ToInt32(row["y"]), Convert.ToDouble(row["z"]), Convert.ToInt32(row["rot"]), Convert.ToInt32(row["user_id"]), 
                                Convert.ToInt32(row["group_id"]), Convert.ToInt32(row["limited_number"]), Convert.ToInt32(row["limited_stack"]), Convert.ToString(row["wall_pos"]), room));
                        }
                        else
                        {
                            // Item data does not exist anymore.
                        }
                    }
                }
            }
            return items;
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
                        if (PlusEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(Row["base_item"]), out ItemData Data))
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
