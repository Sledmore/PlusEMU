using System.Linq;
using System.Drawing;
using Plus.HabboHotel.Rooms.AI;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Inventory.Pets;
using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class KickPetsCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_kickpets"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Kick all of the pets from the room."; }
        }

        public void Execute(GameClient session, Room room, string[] Params)
        {
            if (!room.CheckRights(session, true))
            {
                session.SendWhisper("Oops, only the room owner can run this command!");
                return;
            }

            if (room.GetRoomUserManager().GetPets().Count == 0)
            {
                session.SendWhisper("Oops, there isn't any pets in here!?");
            }

            foreach (RoomUser bot in room.GetRoomUserManager().GetUserList().ToList())
            {
                if (bot == null)
                    continue;

                if (bot.RidingHorse)
                {
                    RoomUser rider = room.GetRoomUserManager().GetRoomUserByVirtualId(bot.HorseID);
                    if (rider != null)
                    {
                        rider.RidingHorse = false;
                        rider.ApplyEffect(-1);
                        rider.MoveTo(new Point(rider.X + 1, rider.Y + 1));
                    }
                    else
                        bot.RidingHorse = false;
                }

                Pet pet = bot.PetData;
                if (pet != null)
                {
                    return;
                }

                pet.RoomId = 0;
                pet.PlacedInRoom = false;

                room.GetRoomUserManager().RemoveBot(bot.VirtualId, false);

                if (pet.OwnerId != session.GetHabbo().Id)
                {
                    GameClient targetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(pet.OwnerId);
                    if (targetClient != null)
                    {
                        if (targetClient.GetHabbo().GetInventoryComponent().TryAddPet(pet))
                        {
                            targetClient.SendPacket(new PetInventoryComposer(targetClient.GetHabbo().GetInventoryComponent().GetPets()));
                        }
                    }
                }

                if (session.GetHabbo().GetInventoryComponent().TryAddPet(pet))
                {
                    session.SendPacket(new PetInventoryComposer(session.GetHabbo().GetInventoryComponent().GetPets()));
                }

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `bots` SET `room_id` = '0', `x` = '0', `Y` = '0', `Z` = '0' WHERE `id` = '" + pet.PetId + "' LIMIT 1");
                    dbClient.RunQuery("UPDATE `bots_petdata` SET `experience` = '" + pet.experience + "', `energy` = '" + pet.Energy + "', `nutrition` = '" + pet.Nutrition + "', `respect` = '" + pet.Respect + "' WHERE `id` = '" + pet.PetId + "' LIMIT 1");
                }
            }

            session.SendWhisper("All pets have been kicked from the room.");
        }
    }
}