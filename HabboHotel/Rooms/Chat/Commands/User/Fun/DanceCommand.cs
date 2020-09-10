using Plus.Communication.Packets.Outgoing.Rooms.Avatar;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class DanceCommand :IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_dance"; }
        }

        public string Parameters
        {
            get { return "%DanceId%"; }
        }

        public string Description
        {
            get { return "Too lazy to dance the proper way? Do it like this!"; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            RoomUser ThisUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Please enter an ID of a dance.");
                return;
            }

            int DanceId;
            if (int.TryParse(Params[1], out DanceId))
            {
                if (DanceId > 4 || DanceId < 0)
                {
                    Session.SendWhisper("The dance ID must be between 0 and 4!");
                    return;
                }

                Session.GetHabbo().CurrentRoom.SendPacket(new DanceComposer(ThisUser.VirtualId, DanceId));
            }
            else
                Session.SendWhisper("Please enter a valid dance ID.");
        }
    }
}
