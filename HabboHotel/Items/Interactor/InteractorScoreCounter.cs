using System;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms.Games;
using Plus.HabboHotel.Rooms.Games.Teams;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorScoreCounter : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
            if (Item.team == TEAM.None)
                return;

            Item.ExtraData = Item.GetRoom().GetGameManager().Points[Convert.ToInt32( Item.team)].ToString();
            Item.UpdateState(false, true);
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
                OldValue++;
            }
            else if (Request == 2)
            {
                OldValue--;
            }
            else if (Request == 3)
            {
                OldValue = 0;
            }

            Item.ExtraData = OldValue.ToString();
            Item.UpdateState(false, true);
        }

        public void OnWiredTrigger(Item Item)
        {
            int OldValue = 0;

            if (!int.TryParse(Item.ExtraData, out OldValue))
            {
            }

            OldValue++;

            Item.ExtraData = OldValue.ToString();
            Item.UpdateState(false, true);
        }
    }
}