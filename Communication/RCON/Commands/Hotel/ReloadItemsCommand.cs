namespace Plus.Communication.Rcon.Commands.Hotel
{
    class ReloadItemsCommand : IRconCommand
    {
        public string Description
        {
            get { return "This command is used to reload the game items."; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public bool TryExecute(string[] parameters)
        {
            PlusEnvironment.GetGame().GetItemManager().Init();

            return true;
        }
    }
}