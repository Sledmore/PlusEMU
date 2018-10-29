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

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Oops, only the room owner can run this command!");
                return;
            }

            if (Room.GetRoomUserManager().GetPets().Count > 0)
            {
                foreach (RoomUser user in Room.GetRoomUserManager().GetUserList().ToList())
                {
                    if (user == null)
                        continue;

                    if (user.RidingHorse)
                    {
                        RoomUser UserRiding = Room.GetRoomUserManager().GetRoomUserByVirtualId(user.HorseID);
                        if (UserRiding != null)
                        {
                            UserRiding.RidingHorse = false;
                            UserRiding.ApplyEffect(-1);
                            UserRiding.MoveTo(new Point(UserRiding.X + 1, UserRiding.Y + 1));
                        }
                        else
                            user.RidingHorse = false;
                    }

                    Pet pet = user.PetData;
                    if (pet != null)
                    {
                        return;
                    }

                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `bots` SET `room_id` = '0', `x` = '0', `Y` = '0', `Z` = '0' WHERE `id` = '" + pet.PetId + "' LIMIT 1");
                        dbClient.RunQuery("UPDATE `bots_petdata` SET `experience` = '" + pet.experience + "', `energy` = '" + pet.Energy + "', `nutrition` = '" + pet.Nutrition + "', `respect` = '" + pet.Respect + "' WHERE `id` = '" + pet.PetId + "' LIMIT 1");
                    }


                    if (pet.OwnerId != Session.GetHabbo().Id)
                    {
                        GameClient Target = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(pet.OwnerId);
                        if (Target != null)
                        {
                            if (Target.GetHabbo().GetInventoryComponent().TryAddPet(pet))
                            {
                                pet.RoomId = 0;
                                pet.PlacedInRoom = false;

                                Room.GetRoomUserManager().RemoveBot(user.VirtualId, false);

                                Target.SendPacket(new PetInventoryComposer(Target.GetHabbo().GetInventoryComponent().GetPets()));
                                return;
                            }
                        }
                    }

                    if (Session.GetHabbo().GetInventoryComponent().TryAddPet(pet))
                    {
                        pet.RoomId = 0;
                        pet.PlacedInRoom = false;

                        Room.GetRoomUserManager().RemoveBot(user.VirtualId, false);
                        Session.SendPacket(new PetInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetPets()));
                    }
                }
                Session.SendWhisper("Success, removed all pets.");
            }
            else
            {
                Session.SendWhisper("Oops, there isn't any pets in here!?");
            }
        }
    }
}