using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Quests;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Items.Interactor
{
    class InteractorSwitch : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {

        }

        public void OnRemove(GameClient Session, Item Item)
        {

        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (Session == null)
                return;

            RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Gamemap.TilesTouching(Item.GetX, Item.GetY, User.X, User.Y))
            {
                int Modes = Item.GetBaseItem().Modes - 1;

                if (Modes <= 0)
                    return;

                PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FurniSwitch);

                int CurrentMode = 0;
                int NewMode = 0;

                if (!int.TryParse(Item.ExtraData, out CurrentMode))
                {
                }

                if (CurrentMode <= 0)
                    NewMode = 1;
                else if (CurrentMode >= Modes)
                    NewMode = 0;
                else
                    NewMode = CurrentMode + 1;

                Item.ExtraData = NewMode.ToString();
                Item.UpdateState();
            }
            else
                User.MoveTo(Item.SquareInFront);
        }

        public void OnWiredTrigger(Item Item)
        {
          
        }
    }
}