namespace Plus.Communication.RCON.Commands.Hotel
{
    class ReloadFilterCommand : IRCONCommand
    {
        public string Description
        {
            get { return "This command is used to reload the chatting filter manager."; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public bool TryExecute(string[] parameters)
        {
            PlusEnvironment.GetGame().GetChatManager().GetFilter().Init();
            return true;
        }
    }
}