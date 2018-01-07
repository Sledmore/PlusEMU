using Plus.HabboHotel.Rooms.Games.Teams;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class EnableCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_enable"; }
        }

        public string Parameters
        {
            get { return "%EffectId%"; }
        }

        public string Description
        {
            get { return "Gives you the ability to set an effect on your user!"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("You must enter an effect ID!");
                return;
            }

            if (!Room.EnablesEnabled && !Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                Session.SendWhisper("Oops, it appears that the room owner has disabled the ability to use the enable command in here.");
                return;
            }

            RoomUser ThisUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);
            if (ThisUser == null)
                return;

            if (ThisUser.RidingHorse)
            {
                Session.SendWhisper("You cannot enable effects whilst riding a horse!");
                return;
            }
            else if (ThisUser.Team != Team.None)
                return;
            else if (ThisUser.isLying)
                return;

            int EffectId = 0;
            if (!int.TryParse(Params[1], out EffectId))
                return;

            if (EffectId > int.MaxValue || EffectId < int.MinValue)
                return;

            if ((EffectId == 102 || EffectId == 187) && !Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                Session.SendWhisper("Sorry, only staff members can use this effects.");
                return;
            }

            if (EffectId == 178 && (!Session.GetHabbo().GetPermissions().HasRight("gold_vip") && !Session.GetHabbo().GetPermissions().HasRight("events_staff")))
            {
                Session.SendWhisper("Sorry, only Gold VIP and Events Staff members can use this effect.");
                return;
            }

            Session.GetHabbo().Effects().ApplyEffect(EffectId);
        }
    }
}
