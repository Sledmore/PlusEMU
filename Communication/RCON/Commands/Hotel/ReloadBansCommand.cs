namespace Plus.Communication.RCON.Commands.Hotel
{
    class ReloadBansCommand : IRCONCommand
    {
        public string Description
        {
            get { return "This command is used to re-cache the bans."; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public bool TryExecute(string[] parameters)
        {
            PlusEnvironment.GetGame().GetModerationManager().ReCacheBans();

            return true;
        }
    }
}