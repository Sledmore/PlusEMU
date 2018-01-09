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

            int petId = packet.PopInt();
            bool type = packet.PopBoolean();

            if (!room.GetRoomUserManager().TryGetPet(petId, out RoomUser pet))
                return;

            if (pet.PetData == null)
                return;

            if (pet.PetData.AnyoneCanRide == 0 && pet.PetData.OwnerId != user.UserId)
            {
                session.SendNotification(
                    "You are unable to ride this horse.\nThe owner of the pet has not selected for anyone to ride it.");
                return;
            }

            if (type)
            {
                if (pet.RidingHorse)
                {
                    string[] speech2 = PlusEnvironment.GetGame().GetChatManager().GetPetLocale().GetValue("pet.alreadymounted");
                    var randomSpeech2 = new Random();
                    pet.Chat(speech2[randomSpeech2.Next(0, speech2.Length - 1)]);
                }
                else if (user.RidingHorse)
                {
                    session.SendNotification("You are already riding a horse!");
                }
                else
                {
                    if (pet.Statusses.Count > 0)
                        pet.Statusses.Clear();

                    int newX2 = user.X;
                    int newY2 = user.Y;
                    room.SendPacket(room.GetRoomItemHandler().UpdateUserOnRoller(pet, new Point(newX2, newY2), 0, room.GetGameMap().SqAbsoluteHeight(newX2, newY2)));
                    room.SendPacket(room.GetRoomItemHandler().UpdateUserOnRoller(user, new Point(newX2, newY2), 0, room.GetGameMap().SqAbsoluteHeight(newX2, newY2) + 1));

                    user.MoveTo(newX2, newY2);

                    pet.ClearMovement(true);

                    user.RidingHorse = true;
                    pet.RidingHorse = true;
                    pet.HorseID = user.VirtualId;
                    user.HorseID = pet.VirtualId;

                    user.ApplyEffect(77);

                    user.RotBody = pet.RotBody;
                    user.RotHead = pet.RotHead;

                    user.UpdateNeeded = true;
                    pet.UpdateNeeded = true;
                }
            }
            else
            {
                if (user.VirtualId == pet.HorseID)
                {
                    pet.Statusses.Remove("sit");
                    pet.Statusses.Remove("lay");
                    pet.Statusses.Remove("snf");
                    pet.Statusses.Remove("eat");
                    pet.Statusses.Remove("ded");
                    pet.Statusses.Remove("jmp");
                    user.RidingHorse = false;
                    user.HorseID = 0;
                    pet.RidingHorse = false;
                    pet.HorseID = 0;
                    user.MoveTo(new Point(user.X + 2, user.Y + 2));
                    user.ApplyEffect(-1);
                    user.UpdateNeeded = true;
                    pet.UpdateNeeded = true;
                }
                else
                {
                    session.SendNotification("Could not dismount this horse - You are not riding it!");
                }
            }

            room.SendPacket(new PetHorseFigureInformationComposer(pet));
        }
    }
}