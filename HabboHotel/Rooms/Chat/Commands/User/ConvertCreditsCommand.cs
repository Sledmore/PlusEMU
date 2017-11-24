using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Data;

using Plus.HabboHotel.Items;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class ConvertCreditsCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_convert_credits"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Convert your exchangeable furniture into actual credits."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            int TotalValue = 0;

            try 
            {
                DataTable Table = null;           
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT `id` FROM `items` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND (`room_id`=  '0' OR `room_id` = '')");
                    Table = dbClient.GetTable();
                }

                if (Table == null)
                {
                    Session.SendWhisper("You currently have no items in your inventory!");
                    return;
                }
                if (Table.Rows.Count > 0)
                {
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        foreach (DataRow Row in Table.Rows)
                        {
                            Item Item = Session.GetHabbo().GetInventoryComponent().GetItem(Convert.ToInt32(Row[0]));
                            if (Item == null || Item.RoomId > 0 || Item.Data.InteractionType != InteractionType.EXCHANGE)
                                continue;
                            
                            int Value = Item.Data.BehaviourData;

                            dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + Item.Id + "' LIMIT 1");

                            Session.GetHabbo().GetInventoryComponent().RemoveItem(Item.Id);

                            TotalValue += Value;

                            if (Value > 0)
                            {
                                Session.GetHabbo().Credits += Value;
                                Session.SendPacket(new CreditBalanceComposer(Session.GetHabbo().Credits));
                            }
                        }
                    }
                }

                if (TotalValue > 0)
                    Session.SendNotification("All credits have successfully been converted!\r\r(Total value: " + TotalValue + " credits!");
                else
                    Session.SendNotification("It appears you don't have any exchangeable items!");
            }
            catch
            {
                Session.SendNotification("Oops, an error occoured whilst converting your credits!");
            }
        }
    }
}
