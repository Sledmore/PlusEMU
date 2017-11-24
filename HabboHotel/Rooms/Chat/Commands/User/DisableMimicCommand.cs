using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Plus.Database.Interfaces;



namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class DisableMimicCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_disable_mimic"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Allows you to disable the ability to be mimiced or to enable the ability to be mimiced."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AllowMimic = !Session.GetHabbo().AllowMimic;
            Session.SendWhisper("You're " + (Session.GetHabbo().AllowMimic == true ? "now" : "no longer") + " able to be mimiced.");

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `allow_mimic` = @AllowMimic WHERE `id` = '" + Session.GetHabbo().Id + "'");
                dbClient.AddParameter("AllowMimic", PlusEnvironment.BoolToEnum(Session.GetHabbo().AllowMimic));
                dbClient.RunQuery();
            }
        }
    }
}