using Plus.HabboHotel.Rooms.AI;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Inventory.Pets;

using System.Drawing;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Database.Interfaces;

namespace Plus.Communication.Packets.Incoming.Rooms.AI.Pets
{
    class PickUpPetEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            if (session.GetHabbo() == null || session.GetHabbo().GetInventoryComponent() == null)
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;

            int petId = packet.PopInt();

            if (!room.GetRoomUserManager().TryGetPet(petId, out RoomUser pet))
            {
                //Check kick rights, just because it seems most appropriate.
                if ((!room.CheckRights(session) && room.WhoCanKick != 2 && room.Group == null) || (room.Group != null && !room.CheckRights(session, false, true)))
                    return;

                //Okay so, we've established we have no pets in this room by this virtual Id, let us check out users, maybe they're creeping as a pet?!
                RoomUser targetUser = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(petId);
                if (targetUser == null)
                    return;

                //Check some values first, please!
                if (targetUser.GetClient() == null || targetUser.GetClient().GetHabbo() == null)
                    return;

                //Update the targets PetId.
                targetUser.GetClient().GetHabbo().PetId = 0;

                //Quickly remove the old user instance.
                room.SendPacket(new UserRemoveComposer(targetUser.VirtualId));

                //Add the new one, they won't even notice a thing!!11 8-)
                room.SendPacket(new UsersComposer(targetUser));
                return;
            }

            if (session.GetHabbo().Id != pet.PetData.OwnerId && !room.CheckRights(session, true))
            {
                session.SendWhisper("You can only pickup your own pets, to kick a pet you must have room rights.");
                return;
            }

            if (pet.RidingHorse)
            {
                RoomUser userRiding = room.GetRoomUserManager().GetRoomUserByVirtualId(pet.HorseID);
                if (userRiding != null)
                {
                    userRiding.RidingHorse = false;
                    userRiding.ApplyEffect(-1);
                    userRiding.MoveTo(new Point(userRiding.X + 1, userRiding.Y + 1));
                }
                else
                    pet.RidingHorse = false;
            }

            Pet data = pet.PetData;
            if (data != null)
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `bots` SET `room_id` = '0', `x` = '0', `Y` = '0', `Z` = '0' WHERE `id` = '" + data.PetId + "' LIMIT 1");
                    dbClient.RunQuery("UPDATE `bots_petdata` SET `experience` = '" + data.experience + "', `energy` = '" + data.Energy + "', `nutrition` = '" + data.Nutrition + "', `respect` = '" + data.Respect + "' WHERE `id` = '" + data.PetId + "' LIMIT 1");
                }
            }

            if (data.OwnerId != session.GetHabbo().Id)
            {
                GameClient target = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(data.OwnerId);
                if (target != null)
                {
                    if (target.GetHabbo().GetInventoryComponent().TryAddPet(pet.PetData))
                    {
                        pet.PetData.RoomId = 0;
                        pet.PetData.PlacedInRoom = false;

                        room.GetRoomUserManager().RemoveBot(pet.VirtualId, false);

                        target.SendPacket(new PetInventoryComposer(target.GetHabbo().GetInventoryComponent().GetPets()));
                        return;
                    }
                }
            }
            
            room.GetRoomUserManager().RemoveBot(pet.VirtualId, false);
        }
    }
}
