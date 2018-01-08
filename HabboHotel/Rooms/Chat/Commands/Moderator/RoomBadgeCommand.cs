using System.Linq;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class RoomBadgeCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_room_badge"; }
        }

        public string Parameters
        {
            get { return "%badge%"; }
        }

        public string Description
        {
            get { return "Give a badge to the entire room!"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please enter the name of the badge you'd like to give to the room.");
                return;
            }

            foreach (RoomUser User in Room.GetRoomUserManager().GetUserList().ToList())
            {
                if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                    continue;

                if (!User.GetClient().GetHabbo().GetBadgeComponent().HasBadge(Params[1]))
                {
                    User.GetClient().GetHabbo().GetBadgeComponent().GiveBadge(Params[1], true, User.GetClient());
                    User.GetClient().SendNotification("You have just been given a badge!");
                }
                else
                    User.GetClient().SendWhisper(Session.GetHabbo().Username + " tried to give you a badge, but you already have it!");
            }

            Session.SendWhisper("You have successfully given every user in this room the " + Params[2] + " badge!");
        }
    }
}
