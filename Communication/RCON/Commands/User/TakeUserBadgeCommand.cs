using System;

using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Rcon.Commands.User
{
    class TakeUserBadgeCommand : IRconCommand
    {
        public string Description
        {
            get { return "This command is used to take a badge from a user."; }
        }

        public string Parameters
        {
            get { return "%userId% %badgeId%"; }
        }

        public bool TryExecute(string[] parameters)
        {
            int userId = 0;
            if (!int.TryParse(parameters[0].ToString(), out userId))
                return false;

            GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(userId);
            if (client == null || client.GetHabbo() == null)
                return false;

            // Validate the badge
            if (string.IsNullOrEmpty(Convert.ToString(parameters[1])))
                return false;

            string badge = Convert.ToString(parameters[1]);

            if (client.GetHabbo().GetBadgeComponent().HasBadge(badge))
            {
                client.GetHabbo().GetBadgeComponent().RemoveBadge(badge);
            }
            return true;
        }
    }
}