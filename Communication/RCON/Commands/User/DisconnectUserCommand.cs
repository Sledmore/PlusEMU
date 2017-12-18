using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Rcon.Commands.User
{
    class DisconnectUserCommand : IRconCommand
    {
        public string Description
        {
            get { return "This command is used to disconnect a user."; }
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

            client.Disconnect();
            return true;
        }
    }
}
