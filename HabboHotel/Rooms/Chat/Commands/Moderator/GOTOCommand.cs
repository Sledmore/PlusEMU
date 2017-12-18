using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GOTOCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_goto"; }
        }

        public string Parameters
        {
            get { return "%room_id%"; }
        }

        public string Description
        {
            get { return ""; }
        }

        public void Execute(GameClients.GameClient session, Rooms.Room room, string[] Params)
        {
            if (Params.Length == 1)
            {
                session.SendWhisper("You must specify a room id!");
                return;
            }


            if (!int.TryParse(Params[1], out int roomId))
                session.SendWhisper("You must enter a valid room ID");
            else
            {
                RoomData Data = null;
                if (!RoomFactory.TryGetData(roomId, out Data))
                {
                    session.SendWhisper("This room does not exist!");
                    return;
                }
                else
                {
                    session.GetHabbo().PrepareRoom(roomId, "");
                }
            }
        }
    }
}