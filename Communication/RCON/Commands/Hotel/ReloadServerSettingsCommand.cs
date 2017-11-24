using Plus.Core;

namespace Plus.Communication.RCON.Commands.Hotel
{
    class ReloadServerSettingsCommand : IRCONCommand
    {
        public string Description
        {
            get { return "This command is used to reload the server settings."; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public bool TryExecute(string[] parameters)
        {
            PlusEnvironment.GetSettingsManager().Init();
            return true;
        }
    }
}