namespace Plus.Communication.Rcon.Commands.Hotel
{
    class ReloadQuestsCommand : IRconCommand
    {
        public string Description
        {
            get { return "This command is used to reload the quests manager."; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public bool TryExecute(string[] parameters)
        {
            PlusEnvironment.GetGame().GetQuestManager().Init();

            return true;
        }
    }
}