using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class CarryCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_carry"; }
        }

        public string Parameters
        {
            get { return "%ItemId%"; }
        }

        public string Description
        {
            get { return "Allows you to carry a hand item"; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            int ItemId = 0;
            if (!int.TryParse(Convert.ToString(Params[1]), out ItemId))
            {
                Session.SendWhisper("Please enter a valid integer.");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            User.CarryItem(ItemId);
        }
    }
}
