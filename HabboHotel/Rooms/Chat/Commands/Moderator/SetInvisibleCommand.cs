using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    public class SetInvisibleCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_invisible"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Become invisible."; }
        }

        public void Execute(GameClient session, Room room, string[] args)
        {
            Habbo habbo = session.GetHabbo();
            
            if (habbo == null || room == null) return;

            if(!habbo.IsInvisible)
            {
                session.SendWhisper("Você está Invisível.");

                habbo.IsInvisible = true;
            }
            else
            {
                if(habbo.InRoom)
                {
                    habbo.PrepareRoom(room.Id, room.Password);
                }
                habbo.IsInvisible = false;
            }
        }
    }
}
