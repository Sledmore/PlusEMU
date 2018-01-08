namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class UnloadCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_unload"; }
        }

        public string Parameters
        {
            get { return "%id%"; }
        }

        public string Description
        {
            get { return "Unload the current room."; }
        }

        public void Execute(GameClients.GameClient session, Room room, string[] Params)
        {
            if (room.CheckRights(session, true) || session.GetHabbo().GetPermissions().HasRight("room_unload_any"))
            {
                PlusEnvironment.GetGame().GetRoomManager().UnloadRoom(room.Id);
            }
        }
    }
}
