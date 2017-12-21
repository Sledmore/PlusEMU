using System;
using System.Drawing;
using Plus.HabboHotel.Rooms;

using Plus.Communication.Packets.Outgoing.Rooms.AI.Pets;

namespace Plus.Communication.Packets.Incoming.Rooms.AI.Pets.Horse
{
    class RideHorseEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;

            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
            if (user == null)
                return;

            int PetId = packet.PopInt();
            bool Type = packet.PopBoolean();

            if (!room.GetRoomUserManager().TryGetPet(PetId, out RoomUser Pet))
                return;

            if (Pet.PetData == null)
                return;

            if (Pet.PetData.AnyoneCanRide == 0 && Pet.PetData.OwnerId != user.UserId)
            {
                session.SendNotification(
                    "You are unable to ride this horse.\nThe owner of the pet has not selected for anyone to ride it.");
                return;
            }

            if (Type)
            {
                if (Pet.RidingHorse)
                {
                    string[] Speech2 = PlusEnvironment.GetGame().GetChatManager().GetPetLocale().GetValue("pet.alreadymounted");
                    var RandomSpeech2 = new Random();
                    Pet.Chat(Speech2[RandomSpeech2.Next(0, Speech2.Length - 1)]);
                }
                else if (user.RidingHorse)
                {
                    session.SendNotification("You are already riding a horse!");
                }
                else
                {
                    if (Pet.Statusses.Count > 0)
                        Pet.Statusses.Clear();

                    int NewX2 = user.X;
                    int NewY2 = user.Y;
                    room.SendPacket(room.GetRoomItemHandler().UpdateUserOnRoller(Pet, new Point(NewX2, NewY2), 0, room.GetGameMap().SqAbsoluteHeight(NewX2, NewY2)));
                    room.SendPacket(room.GetRoomItemHandler().UpdateUserOnRoller(user, new Point(NewX2, NewY2), 0, room.GetGameMap().SqAbsoluteHeight(NewX2, NewY2) + 1));

                    user.MoveTo(NewX2, NewY2);

                    Pet.ClearMovement(true);

                    user.RidingHorse = true;
                    Pet.RidingHorse = true;
                    Pet.HorseID = user.VirtualId;
                    user.HorseID = Pet.VirtualId;

                    user.ApplyEffect(77);

                    user.RotBody = Pet.RotBody;
                    user.RotHead = Pet.RotHead;

                    user.UpdateNeeded = true;
                    Pet.UpdateNeeded = true;
                }
            }
            else
            {
                if (user.VirtualId == Pet.HorseID)
                {
                    Pet.Statusses.Remove("sit");
                    Pet.Statusses.Remove("lay");
                    Pet.Statusses.Remove("snf");
                    Pet.Statusses.Remove("eat");
                    Pet.Statusses.Remove("ded");
                    Pet.Statusses.Remove("jmp");
                    user.RidingHorse = false;
                    user.HorseID = 0;
                    Pet.RidingHorse = false;
                    Pet.HorseID = 0;
                    user.MoveTo(new Point(user.X + 2, user.Y + 2));
                    user.ApplyEffect(-1);
                    user.UpdateNeeded = true;
                    Pet.UpdateNeeded = true;
                }
                else
                {
                    session.SendNotification("Could not dismount this horse - You are not riding it!");
                }
            }

            room.SendPacket(new PetHorseFigureInformationComposer(Pet));
        }
    }
}