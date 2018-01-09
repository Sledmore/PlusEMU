using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Rcon.Commands.User
{
    class DisconnectUserCommand : IRconCommand
    {
        public string Description
        {
            get { return "This command is used to Disconnect a user."; }
        }

        public string Parameters
        {
            get { return "%userId%"; }
        }

        public bool TryExecute(string[] parameters)
        {
            if (!int.TryParse(parameters[0], out int userId))
                return false;

            GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(userId);
            if (client == null || client.GetHabbo() == null)
                return false;

            client.Disconnect();
            return true;
        }
    }
}
