using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class UnloadCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_unload"; }
        }

        public string Parameters
        {
            get { return "%id%"; }
        }

        public string Description
        {
            get { return "Unload the current room."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Session.GetHabbo().GetPermissions().HasRight("room_unload_any"))
            {
                Room R = null;
                if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(Room.Id, out R))
                    return;

                PlusEnvironment.GetGame().GetRoomManager().UnloadRoom(R, true);
            }
            else
            {
                if (Room.CheckRights(Session, true))
                {
                    PlusEnvironment.GetGame().GetRoomManager().UnloadRoom(Room);
                }
            }
        }
    }
}
