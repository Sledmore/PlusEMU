namespace Plus.Communication.Rcon.Commands.Hotel
{
    class ReloadServerSettingsCommand : IRconCommand
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