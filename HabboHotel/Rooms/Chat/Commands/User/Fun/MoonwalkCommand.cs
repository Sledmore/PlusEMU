namespace Plus.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class MoonwalkCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_moonwalk"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Wear the shoes of Michael Jackson."; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            User.moonwalkEnabled = !User.moonwalkEnabled;

            if (User.moonwalkEnabled)
                Session.SendWhisper("Moonwalk enabled!");
            else
                Session.SendWhisper("Moonwalk disabled!");
        }
    }
}
