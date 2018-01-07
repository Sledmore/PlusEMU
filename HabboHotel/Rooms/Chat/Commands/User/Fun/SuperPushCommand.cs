using System;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class SuperPushCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_super_push"; }
        }

        public string Parameters
        {
            get { return "%target%"; }
        }

        public string Description
        {
            get { return "Superpush another user. (Pushes them 3 squares away)"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please enter the username of the user you wish to push.");
                return;
            }

            if (!Room.SuperPushEnabled && !Room.CheckRights(Session, true) && !Session.GetHabbo().GetPermissions().HasRight("room_override_custom_config"))
            {
                Session.SendWhisper("Oops, it appears that the room owner has disabled the ability to use the push command in here.");
                return;
            }

            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("An error occoured whilst finding that user, maybe they're not online.");
                return;
            }

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (TargetUser == null)
            {
                Session.SendWhisper("An error occoured whilst finding that user, maybe they're not online or in this room.");
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Come on, surely you don't want to push yourself!");
                return;
            }

            if (TargetUser.TeleportEnabled)
            {
                Session.SendWhisper("Oops, you cannot push a user whilst they have their teleport mode enabled.");
                return;
            }

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;

            if (!((Math.Abs(TargetUser.X - ThisUser.X) >= 2) || (Math.Abs(TargetUser.Y - ThisUser.Y) >= 2)))
            {
                if (TargetUser.SetX - 1 == Room.GetGameMap().Model.DoorX || TargetUser.SetY - 1 == Room.GetGameMap().Model.DoorY)
                {
                    Session.SendWhisper("Please don't push that user out of the room :(!");
                    return;
                }

                if (TargetUser.SetX - 2 == Room.GetGameMap().Model.DoorX || TargetUser.SetY - 2 == Room.GetGameMap().Model.DoorY)
                {
                    Session.SendWhisper("Please don't push that user out of the room :(!");
                    return;
                }

                if (TargetUser.SetX - 3 == Room.GetGameMap().Model.DoorX || TargetUser.SetY - 3 == Room.GetGameMap().Model.DoorY)
                {
                    Session.SendWhisper("Please don't push that user out of the room :(!");
                    return;
                }

                if (TargetUser.RotBody == 4)
                {
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y + 3);
                }

                if (ThisUser.RotBody == 0)
                {
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y - 3);
                }

                if (ThisUser.RotBody == 6)
                {
                    TargetUser.MoveTo(TargetUser.X - 3, TargetUser.Y);
                }

                if (ThisUser.RotBody == 2)
                {
                    TargetUser.MoveTo(TargetUser.X + 3, TargetUser.Y);
                }

                if (ThisUser.RotBody == 3)
                {
                    TargetUser.MoveTo(TargetUser.X + 3, TargetUser.Y);
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y + 3);
                }

                if (ThisUser.RotBody == 1)
                {
                    TargetUser.MoveTo(TargetUser.X + 3, TargetUser.Y);
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y - 3);
                }

                if (ThisUser.RotBody == 7)
                {
                    TargetUser.MoveTo(TargetUser.X - 3, TargetUser.Y);
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y - 3);
                }

                if (ThisUser.RotBody == 5)
                {
                    TargetUser.MoveTo(TargetUser.X - 3, TargetUser.Y);
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y + 3);
                }

                Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "*super pushes " + Params[1] + "*", 0, ThisUser.LastBubble));
            }
            else
            {
                Session.SendWhisper("Oops, " + Params[1] + " is not close enough!");
            }
        }
    }
}
