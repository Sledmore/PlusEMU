using Plus.Communication.Packets.Outgoing.Catalog;

namespace Plus.Communication.Rcon.Commands.Hotel
{
    class ReloadCatalogCommand : IRconCommand
    {
        public string Description
        {
            get { return "This command is used to reload the catalog."; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public bool TryExecute(string[] parameters)
        {
            PlusEnvironment.GetGame().GetCatalog().Init(PlusEnvironment.GetGame().GetItemManager());
            PlusEnvironment.GetGame().GetClientManager().SendPacket(new CatalogUpdatedComposer());
            return true;
        }
    }
}