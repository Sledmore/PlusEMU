using System;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Rcon.Commands.User
{
    class ProgressUserAchievementCommand : IRconCommand
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
            if (!int.TryParse(parameters[0], out int userId))
                return false;

            GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(userId);
            if (client == null || client.GetHabbo() == null)
                return false;
        
            // Validate the achievement
            if (string.IsNullOrEmpty(Convert.ToString(parameters[1])))
                return false;

            string achievement = Convert.ToString(parameters[1]);

            // Validate the progress
            if (!int.TryParse(parameters[2], out int progress))
                return false;

            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(client, achievement, progress);
            return true;
        }
    }
}