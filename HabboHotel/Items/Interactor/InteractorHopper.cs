

using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorHopper : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
            Item.GetRoom().GetRoomItemHandler().HopperCount++;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO items_hopper (hopper_id, room_id) VALUES (@hopperid, @roomid);");
                dbClient.AddParameter("hopperid", Item.Id);
                dbClient.AddParameter("roomid", Item.RoomId);
                dbClient.RunQuery();
            }

            if (Item.InteractingUser != 0)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Item.InteractingUser);

                if (User != null)
                {
                    User.ClearMovement(true);
                    User.AllowOverride = false;
                    User.CanWalk = true;
                }

                Item.InteractingUser = 0;
            }
        }

        public void OnRemove(GameClient Session, Item Item)
        {
            Item.GetRoom().GetRoomItemHandler().HopperCount--;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM items_hopper WHERE item_id=@hid OR room_id=" + Item.GetRoom().RoomId +
                                  " LIMIT 1");
                dbClient.AddParameter("hid", Item.Id);
                dbClient.RunQuery();
            }

            if (Item.InteractingUser != 0)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Item.InteractingUser);

                if (User != null)
                {
                    User.UnlockWalking();
                }

                Item.InteractingUser = 0;
            }
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (Item == null || Item.GetRoom() == null || Session == null || Session.GetHabbo() == null)
                return;
            RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (User == null)
            {
                return;
            }

            // Alright. But is this user in the right position?
            if (User.Coordinate == Item.Coordinate || User.Coordinate == Item.SquareInFront)
            {
                // Fine. But is this tele even free?
                if (Item.InteractingUser != 0)
                {
                    return;
                }

                User.TeleDelay = 2;
                Item.InteractingUser = User.GetClient().GetHabbo().Id;
            }
            else if (User.CanWalk)
            {
                User.MoveTo(Item.SquareInFront);
            }
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}