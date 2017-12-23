using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Items.Interactor
{
    public interface IFurniInteractor
    {
        void OnPlace(GameClient session, Item item);
        void OnRemove(GameClient session, Item item);
        void OnTrigger(GameClient session, Item item, int request, bool hasRights);
        void OnWiredTrigger(Item item);
    }
}