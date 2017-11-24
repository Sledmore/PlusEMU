using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.RCON.Commands.User
{
    class ReloadUserRankCommand : IRCONCommand
    {
        public string Description
        {
            get { return "This command is used to reload a users rank and permissions."; }
        }

        public string Parameters
        {
            get { return "%userId%"; }
        }

        public bool TryExecute(string[] parameters)
        {
            int userId = 0;
            if (!int.TryParse(parameters[0].ToString(), out userId))
                return false;

            GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(userId);
            if (client == null || client.GetHabbo() == null)
                return false;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `rank` FROM `users` WHERE `id` = @userId LIMIT 1");
                dbClient.AddParameter("userId", userId);
                client.GetHabbo().Rank = dbClient.GetInteger();
            }

            client.GetHabbo().GetPermissions().Init(client.GetHabbo());

            if (client.GetHabbo().GetPermissions().HasRight("mod_tickets"))
            {
                client.SendPacket(new ModeratorInitComposer(
                  PlusEnvironment.GetGame().GetModerationManager().UserMessagePresets,
                  PlusEnvironment.GetGame().GetModerationManager().RoomMessagePresets,
                  PlusEnvironment.GetGame().GetModerationManager().GetTickets));
            }
            return true;
        }
    }
}