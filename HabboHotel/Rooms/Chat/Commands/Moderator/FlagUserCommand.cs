using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Handshake;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class FlagUserCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_flaguser"; }
        }

        public string Parameters
        {
            get { return "%username%"; }
        }

        public string Description
        {
            get { return "Forces the specified user to change their name."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please enter the username you wish to flag.");
                return;
            }

            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("An error occoured whilst finding that user, maybe they're not online.");
                return;
            }

            if (TargetClient.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                Session.SendWhisper("You are not allowed to flag that user.");
                return;
            }
            else
            {
                TargetClient.GetHabbo().LastNameChange = 0;
                TargetClient.GetHabbo().ChangingName = true;
                TargetClient.SendNotification("Please be aware that if your username is deemed as inappropriate, you will be banned without question.\r\rAlso note that Staff will NOT allow you to change your username again should you have an issue with what you have chosen.\r\rClose this window and click yourself to begin choosing a new username!");
                TargetClient.SendPacket(new UserObjectComposer(TargetClient.GetHabbo()));
            }

        }
    }
}
