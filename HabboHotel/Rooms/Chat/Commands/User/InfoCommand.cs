using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

using Plus.Communication.Packets.Outgoing.Users;
using Plus.Communication.Packets.Outgoing.Notifications;


using Plus.Communication.Packets.Outgoing.Handshake;
using Plus.Communication.Packets.Outgoing.Quests;
using Plus.HabboHotel.Items;
using Plus.Communication.Packets.Outgoing.Inventory.Furni;
using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.HabboHotel.Quests;
using Plus.HabboHotel.Rooms;
using System.Threading;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using Plus.Communication.Packets.Outgoing.Pets;
using Plus.Communication.Packets.Outgoing.Messenger;
using Plus.HabboHotel.Users.Messenger;
using Plus.Communication.Packets.Outgoing.Rooms.Polls;
using Plus.Communication.Packets.Outgoing.Rooms.Notifications;
using Plus.Communication.Packets.Outgoing.Availability;
using Plus.Communication.Packets.Outgoing;
using Plus.Communication.Packets.Outgoing.Rooms.Polls.Questions;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class InfoCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_info"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Displays generic information that everybody loves to see."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            TimeSpan Uptime = DateTime.Now - PlusEnvironment.ServerStarted;
            int OnlineUsers = PlusEnvironment.GetGame().GetClientManager().Count;
            int RoomCount = PlusEnvironment.GetGame().GetRoomManager().Count;

            Session.SendPacket(new RoomNotificationComposer("Powered by PlusEmulator",
                 "<b>Credits</b>:\n" +
                 "DevBest Community\n\n" +
                 "<b>Current run time information</b>:\n" +
                 "Online Users: " + OnlineUsers + "\n" +
                 "Rooms Loaded: " + RoomCount + "\n" +
                 "Uptime: " + Uptime.Days + " day(s), " + Uptime.Hours + " hours and " + Uptime.Minutes + " minutes.\n\n" +
                 "<b>SWF Revision</b>:\n" + PlusEnvironment.SWFRevision, "plus", ""));
        }
    }
}