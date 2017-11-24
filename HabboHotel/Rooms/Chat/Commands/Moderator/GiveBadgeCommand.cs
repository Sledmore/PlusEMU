using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GiveBadgeCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_give_badge"; }
        }

        public string Parameters
        {
            get { return "%username% %badge%"; }
        }

        public string Description
        {
            get { return "Give a badge to another user."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length != 3)
            {
                Session.SendWhisper("Please enter a username and the code of the badge you'd like to give!");
                return;
            }

            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient != null)
            {
                if (!TargetClient.GetHabbo().GetBadgeComponent().HasBadge(Params[2]))
                {
                    TargetClient.GetHabbo().GetBadgeComponent().GiveBadge(Params[2], true, TargetClient);
                    if (TargetClient.GetHabbo().Id != Session.GetHabbo().Id)
                        TargetClient.SendNotification("You have just been given a badge!");
                    else
                        Session.SendWhisper("You have successfully given yourself the badge " + Params[2] + "!");
                }
                else
                    Session.SendWhisper("Oops, that user already has this badge (" + Params[2] + ") !");
                return;
            }
            else
            {
                Session.SendWhisper("Oops, we couldn't find that target user!");
                return;
            }
        }
    }
}
