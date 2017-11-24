using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorScoreboard : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (!HasRights)
            {
                return;
            }

            int OldValue = 0;

            if (!int.TryParse(Item.ExtraData, out OldValue))
            {
            }


            if (Request == 1)
            {
                if (Item.pendingReset && OldValue > 0)
                {
                    OldValue = 0;
                    Item.pendingReset = false;
                }
                else
                {
                    OldValue = OldValue + 60;
                    Item.UpdateNeeded = false;
                }
            }
            else if (Request == 2)
            {
                Item.UpdateNeeded = !Item.UpdateNeeded;
                Item.pendingReset = true;
            }


            Item.ExtraData = OldValue.ToString();
            Item.UpdateState();
        }

        public void OnWiredTrigger(Item Item)
        {
            int OldValue = 0;

            if (!int.TryParse(Item.ExtraData, out OldValue))
            {
            }

            OldValue = OldValue + 60;
            Item.UpdateNeeded = false;

            Item.ExtraData = OldValue.ToString();
            Item.UpdateState();
        }
    }
}