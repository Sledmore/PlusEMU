using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Rooms.PathFinding;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorVendor : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";
            Item.UpdateNeeded = true;

            if (Item.InteractingUser > 0)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Item.InteractingUser);

                if (User != null)
                {
                    User.CanWalk = true;
                }
            }
        }

        public void OnRemove(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";

            if (Item.InteractingUser > 0)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Item.InteractingUser);

                if (User != null)
                {
                    User.CanWalk = true;
                }
            }
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (Item.ExtraData != "1" && Item.GetBaseItem().VendingIds.Count >= 1 && Item.InteractingUser == 0 &&
                Session != null)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

                if (User == null)
                {
                    return;
                }

                if (!Gamemap.TilesTouching(User.X, User.Y, Item.GetX, Item.GetY))
                {
                    User.MoveTo(Item.SquareInFront);
                    return;
                }

                Item.InteractingUser = Session.GetHabbo().Id;

                User.CanWalk = false;
                User.ClearMovement(true);
                User.SetRot(Rotation.Calculate(User.X, User.Y, Item.GetX, Item.GetY), false);

                Item.RequestUpdate(2, true);

                Item.ExtraData = "1";
                Item.UpdateState(false, true);
            }
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}