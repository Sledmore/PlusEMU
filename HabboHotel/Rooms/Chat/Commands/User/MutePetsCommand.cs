using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Plus.Database.Interfaces;


namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class MutePetsCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_mute_pets"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Ignore bot chat or enable it again."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AllowPetSpeech = !Session.GetHabbo().AllowPetSpeech;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `users` SET `pets_muted` = '" + ((Session.GetHabbo().AllowPetSpeech) ? 1 : 0) + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }

            if (Session.GetHabbo().AllowPetSpeech)
                Session.SendWhisper("Change successful, you can no longer see speech from pets.");
            else
                Session.SendWhisper("Change successful, you can now see speech from pets.");
        }
    }
}
