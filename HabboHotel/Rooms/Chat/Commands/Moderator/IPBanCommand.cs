using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Utilities;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.GameClients;


using Plus.HabboHotel.Moderation;

using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class IPBanCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_ip_ban"; }
        }

        public string Parameters
        {
            get { return "%username%"; }
        }

        public string Description
        {
            get { return "IP and account ban another user."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please enter the username of the user you'd like to IP ban & account ban.");
                return;
            }

            Habbo Habbo = PlusEnvironment.GetHabboByUsername(Params[1]);
            if (Habbo == null)
            {
                Session.SendWhisper("An error occoured whilst finding that user in the database.");
                return;
            }

            if (Habbo.GetPermissions().HasRight("mod_tool") && !Session.GetHabbo().GetPermissions().HasRight("mod_ban_any"))
            {
                Session.SendWhisper("Oops, you cannot ban that user.");
                return;
            }

            String IPAddress = String.Empty;
            Double Expire = PlusEnvironment.GetUnixTimestamp() + 78892200;
            string Username = Habbo.Username;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `user_info` SET `bans` = `bans` + '1' WHERE `user_id` = '" + Habbo.Id + "' LIMIT 1");

                dbClient.SetQuery("SELECT `ip_last` FROM `users` WHERE `id` = '" + Habbo.Id + "' LIMIT 1");
                IPAddress = dbClient.GetString();
            }

            string Reason = null;
            if (Params.Length >= 3)
                Reason = CommandManager.MergeParams(Params, 2);
            else
                Reason = "No reason specified.";

            if (!string.IsNullOrEmpty(IPAddress))
                PlusEnvironment.GetGame().GetModerationManager().BanUser(Session.GetHabbo().Username, ModerationBanType.IP, IPAddress, Reason, Expire);
            PlusEnvironment.GetGame().GetModerationManager().BanUser(Session.GetHabbo().Username, ModerationBanType.USERNAME, Habbo.Username, Reason, Expire);

            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient != null)
                TargetClient.Disconnect();


            Session.SendWhisper("Success, you have IP and account banned the user '" + Username + "' for '" + Reason + "'!");
        }
    }
}