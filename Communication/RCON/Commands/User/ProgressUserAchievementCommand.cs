using System;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.RCON.Commands.User
{
    class ProgressUserAchievementCommand : IRCONCommand
    {
        public string Description
        {
            get { return "This command is used to progress a users achievement."; }
        }

        public string Parameters
        {
            get { return "%userId% %achievement% %progess%"; }
        }

        public bool TryExecute(string[] parameters)
        {
            int userId = 0;
            if (!int.TryParse(parameters[0].ToString(), out userId))
                return false;

            GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(userId);
            if (client == null || client.GetHabbo() == null)
                return false;
        
            // Validate the achievement
            if (string.IsNullOrEmpty(Convert.ToString(parameters[1])))
                return false;

            string achievement = Convert.ToString(parameters[1]);

            // Validate the progress
            int progress = 0;
            if (!int.TryParse(parameters[2].ToString(), out progress))
                return false;

            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(client, achievement, progress);
            return true;
        }
    }
}