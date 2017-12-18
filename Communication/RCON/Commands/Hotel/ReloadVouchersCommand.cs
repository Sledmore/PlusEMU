namespace Plus.Communication.Rcon.Commands.Hotel
{
    class ReloadVouchersCommand : IRconCommand
    {
        public string Description
        {
            get { return "This command is used to reload the voucher manager."; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public bool TryExecute(string[] parameters)
        {
            PlusEnvironment.GetGame().GetCatalog().GetVoucherManager().Init();

            return true;
        }
    }
}