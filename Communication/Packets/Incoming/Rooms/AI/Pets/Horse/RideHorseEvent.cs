using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms.AI;
using Plus.HabboHotel.Rooms;

using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.AI.Pets;



namespace Plus.Communication.Packets.Incoming.Rooms.AI.Pets.Horse
{
    class RideHorseEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            int PetId = Packet.PopInt();
            bool Type = Packet.PopBoolean();
           
            RoomUser Pet = null;
            if (!Room.GetRoomUserManager().TryGetPet(PetId, out Pet))
                return;

            if (Pet.PetData == null)
                return;
            
            if (Pet.PetData.AnyoneCanRide == 0 && Pet.PetData.OwnerId != User.UserId)
            {
                Session.SendNotification(
                    "You are unable to ride this horse.\nThe owner of the pet has not selected for anyone to ride it.");
                return;
            }

            if (Type)
            {
                if (Pet.RidingHorse)
                {
                    string[] Speech2 = PlusEnvironment.GetGame().GetChatManager().GetPetLocale().GetValue("pet.alreadymounted");
                    var RandomSpeech2 = new Random();
                    Pet.Chat(Speech2[RandomSpeech2.Next(0, Speech2.Length - 1)], false);
                }
                else if (User.RidingHorse)
                {
                    Session.SendNotification("You are already riding a horse!");
                }
                else
                {
                    if (Pet.Statusses.Count > 0)
                        Pet.Statusses.Clear();

                    int NewX2 = User.X;
                    int NewY2 = User.Y;
                    Room.SendPacket(Room.GetRoomItemHandler() .UpdateUserOnRoller(Pet, new Point(NewX2, NewY2), 0, Room.GetGameMap().SqAbsoluteHeight(NewX2, NewY2)));
                    Room.SendPacket(Room.GetRoomItemHandler() .UpdateUserOnRoller(User, new Point(NewX2, NewY2), 0, Room.GetGameMap().SqAbsoluteHeight(NewX2, NewY2) + 1));

                    User.MoveTo(NewX2, NewY2);
                    
                    Pet.ClearMovement(true);

                    User.RidingHorse = true;
                    Pet.RidingHorse = true;
                    Pet.HorseID = User.VirtualId;
                    User.HorseID = Pet.VirtualId;
                   
                    User.ApplyEffect(77);
            
                    User.RotBody = Pet.RotBody;
                    User.RotHead = Pet.RotHead;
            
                    User.UpdateNeeded = true;
                    Pet.UpdateNeeded = true;

     

                }
            }
            else
            {
                if (User.VirtualId == Pet.HorseID)
                {
                    Pet.Statusses.Remove("sit");
                    Pet.Statusses.Remove("lay");
                    Pet.Statusses.Remove("snf");
                    Pet.Statusses.Remove("eat");
                    Pet.Statusses.Remove("ded");
                    Pet.Statusses.Remove("jmp");
                    User.RidingHorse = false;
                    User.HorseID = 0;
                    Pet.RidingHorse = false;
                    Pet.HorseID = 0;
                    User.MoveTo(new Point(User.X + 2, User.Y + 2));
                    User.ApplyEffect(-1);
                    User.UpdateNeeded = true;
                    Pet.UpdateNeeded = true;
                }
                else
                {
                    Session.SendNotification("Could not dismount this horse - You are not riding it!");
                }
            }

            Room.SendPacket(new PetHorseFigureInformationComposer(Pet));
        }
    }
}
