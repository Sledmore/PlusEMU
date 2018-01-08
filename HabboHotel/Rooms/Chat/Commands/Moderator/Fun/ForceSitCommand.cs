namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class ForceSitCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_forcesit"; }
        }

        public string Parameters
        {
            get { return "%username%"; }
        }

        public string Description
        {
            get { return "Force another to user sit."; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oops, you forgot to choose a target user!");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Params[1]);
            if (User == null)
                return;

            if (User.Statusses.ContainsKey("lie") || User.isLying || User.RidingHorse || User.IsWalking)
                return;

            if (!User.Statusses.ContainsKey("sit"))
            {
                if ((User.RotBody % 2) == 0)
                {
                    if (User == null)
                        return;

                    try
                    {
                        User.Statusses.Add("sit", "1.0");
                        User.Z -= 0.35;
                        User.isSitting = true;
                        User.UpdateNeeded = true;
                    }
                    catch { }
                }
                else
                {
                    User.RotBody--;
                    User.Statusses.Add("sit", "1.0");
                    User.Z -= 0.35;
                    User.isSitting = true;
                    User.UpdateNeeded = true;
                }
            }
            else if (User.isSitting == true)
            {
                User.Z += 0.35;
                User.Statusses.Remove("sit");
                User.Statusses.Remove("1.0");
                User.isSitting = false;
                User.UpdateNeeded = true;
            }
        }
    }
}
