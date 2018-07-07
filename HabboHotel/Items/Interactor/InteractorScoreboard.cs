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

            // Request 1 - Decrease value with red button
            // Request 2 - Increase value with green button
            // Request 3 - Reset with UI/Wired/Double click

            // Find out what number we are on right now
            if (!int.TryParse(Item.ExtraData, out int OldValue))
            {
                OldValue = 0;
            }

            // Decrease value with red button
            if (OldValue >= 0 && OldValue <= 99 && Request == 1)
            {
                if (OldValue > 0)
                    OldValue--;
                else if (OldValue == 0)
                    OldValue = 99;
            }

            // Increase value with green button
            if (OldValue >= 0 && OldValue <= 99 && Request == 2)
            {
                if (OldValue < 99)
                    OldValue++;
                else if (OldValue == 99)
                    OldValue = 0;
            }

            // Reset with UI/Wired/Double click
            if (Request == 3)
            {
                OldValue = 0;
                Item.pendingReset = true;
            }

            Item.ExtraData = OldValue.ToString();
            Item.UpdateState();
        }

        public void OnWiredTrigger(Item Item)
        {
            // Always reset scoreboard on Wired trigger
            Item.ExtraData = "0";
            Item.UpdateState();
        }
    }
}