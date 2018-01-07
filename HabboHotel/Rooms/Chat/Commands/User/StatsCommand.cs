using System;
using System.Text;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class StatsCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_stats"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "View your current statistics."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            double Minutes = Session.GetHabbo().GetStats().OnlineTime / 60;
            double Hours = Minutes / 60;
            int OnlineTime = Convert.ToInt32(Hours);
            string s = OnlineTime == 1 ? "" : "s";

            StringBuilder HabboInfo = new StringBuilder();
            HabboInfo.Append("Your account stats:\r\r");

            HabboInfo.Append("Currency Info:\r");
            HabboInfo.Append("Credits: " + Session.GetHabbo().Credits + "\r");
            HabboInfo.Append("Duckets: " + Session.GetHabbo().Duckets + "\r");
            HabboInfo.Append("Diamonds: " + Session.GetHabbo().Diamonds + "\r");
            HabboInfo.Append("Online Time: " + OnlineTime + " Hour" + s + "\r");
            HabboInfo.Append("Respects: " + Session.GetHabbo().GetStats().Respect + "\r");
            HabboInfo.Append("GOTW Points: " + Session.GetHabbo().GOTWPoints + "\r\r");


            Session.SendNotification(HabboInfo.ToString());
        }
    }
}
