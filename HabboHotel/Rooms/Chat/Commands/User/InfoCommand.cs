using System;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Notifications;

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