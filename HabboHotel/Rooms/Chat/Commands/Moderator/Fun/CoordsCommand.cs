namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class CoordsCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_coords"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Used to get your current position within the room."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;

            Session.SendNotification("X: " + ThisUser.X + "\n - Y: " + ThisUser.Y + "\n - Z: " + ThisUser.Z + "\n - Rot: " + ThisUser.RotBody + ", sqState: " + Room.GetGameMap().GameMap[ThisUser.X, ThisUser.Y].ToString() + "\n\n - RoomID: " + Session.GetHabbo().CurrentRoomId);                           
        }
    }
}
