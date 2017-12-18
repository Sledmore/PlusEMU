namespace Plus.Communication.Rcon.Commands.Hotel
{
    class ReloadNavigatorCommand : IRconCommand
    {
        public string Description
        {
            get { return "This command is used to reload the navigator."; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public bool TryExecute(string[] parameters)
        {
            PlusEnvironment.GetGame().GetNavigator().Init();

            return true;
        }
    }
}