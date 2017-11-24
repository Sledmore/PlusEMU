using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Communication.Packets.Outgoing.Rooms.Engine;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User.Fun 
{
    class PetCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_pet"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Allows you to transform into a pet.."; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            RoomUser RoomUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (RoomUser == null)
                return;

            if (!Room.PetMorphsAllowed)
            {
                Session.SendWhisper("The room owner has disabled the ability to use a pet morph in this room.");
                if (Session.GetHabbo().PetId > 0)
                {
                    Session.SendWhisper("Oops, you still have a morph, un-morphing you.");
                    //Change the users Pet Id.
                    Session.GetHabbo().PetId = 0;

                    //Quickly remove the old user instance.
                    Room.SendPacket(new UserRemoveComposer(RoomUser.VirtualId));

                    //Add the new one, they won't even notice a thing!!11 8-)
                    Room.SendPacket(new UsersComposer(RoomUser));
                }
                return;
            }

            if (Params.Length == 1)
            {
                Session.SendWhisper("Oops, you forgot to choose the type of pet you'd like to turn into! Use :pet list to see the availiable morphs!");
                return;
            }

            if (Params[1].ToString().ToLower() == "list")
            {
                Session.SendWhisper("Habbo, Dog, Cat, Terrier, Croc, Bear, Pig, Lion, Rhino, Spider, Turtle, Chick, Frog, Drag, Monkey, Horse, Bunny, Pigeon, Demon and Gnome.");
                return;
            }

            int TargetPetId = GetPetIdByString(Params[1].ToString());
            if (TargetPetId == 0)
            {
                Session.SendWhisper("Oops, couldn't find a pet by that name!");
                return;
            }

            //Change the users Pet Id.
            Session.GetHabbo().PetId = (TargetPetId == -1 ? 0 : TargetPetId);

            //Quickly remove the old user instance.
            Room.SendPacket(new UserRemoveComposer(RoomUser.VirtualId));

            //Add the new one, they won't even notice a thing!!11 8-)
            Room.SendPacket(new UsersComposer(RoomUser));

            //Tell them a quick message.
            if (Session.GetHabbo().PetId > 0)
                Session.SendWhisper("Use ':pet habbo' to turn back into a Habbo!");
        }

        private int GetPetIdByString(string Pet)
        {
            switch (Pet.ToLower())
            {
                default:
                    return 0;
                case "habbo":
                    return -1;
                case "dog":
                    return 60;//This should be 0.
                case "cat":
                    return 1;
                case "terrier":
                    return 2;
                case "croc":
                case "croco":
                    return 3;
                case "bear":
                    return 4;
                case "liz":
                case "pig":
                case "kill":
                    return 5;
                case "lion":
                case "rawr":
                    return 6;
                case "rhino":
                    return 7;
                case "spider":
                    return 8;
                case "turtle":
                    return 9;
                case "chick":
                case "chicken":
                    return 10;
                case "frog":
                    return 11;
                case "drag":
                case "dragon":
                    return 12;
                case "monkey":
                    return 14;
                case "horse":
                    return 15;
                case "bunny":
                    return 17;
                case "pigeon":
                    return 21;
                case "demon":
                    return 23;
                case "gnome":
                    return 26;
            }
        }
    }
}