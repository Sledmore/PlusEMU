using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorSpinningBottle : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";
            Item.UpdateState(true, false);
        }

        public void OnRemove(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (Item.ExtraData != "-1")
            {
                Item.ExtraData = "-1";
                Item.UpdateState(false, true);
                Item.RequestUpdate(3, true);
            }
        }

        public void OnWiredTrigger(Item Item)
        {
            if (Item.ExtraData != "-1")
            {
                Item.ExtraData = "-1";
                Item.UpdateState(false, true);
                Item.RequestUpdate(3, true);
            }
        }
    }
}