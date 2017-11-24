using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class TeleportCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_teleport"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "The ability to teleport anywhere within the room."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            User.TeleportEnabled = !User.TeleportEnabled;
            Room.GetGameMap().GenerateMaps();
        }
    }
}
