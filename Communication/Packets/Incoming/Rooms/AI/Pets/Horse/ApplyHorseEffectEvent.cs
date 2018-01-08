using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;

using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.AI.Pets;

using Plus.Database.Interfaces;

namespace Plus.Communication.Packets.Incoming.Rooms.AI.Pets.Horse
{
    class ApplyHorseEffectEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;
            
            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;

            int itemId = packet.PopInt();
            Item item = room.GetRoomItemHandler().GetItem(itemId);
            if (item == null)
                return;

            int petId = packet.PopInt();
            
            if (!room.GetRoomUserManager().TryGetPet(petId, out RoomUser petUser))
                return;

            if (petUser.PetData == null || petUser.PetData.OwnerId != session.GetHabbo().Id)
                return;

            if (item.Data.InteractionType == InteractionType.HORSE_SADDLE_1)
            {
                petUser.PetData.Saddle = 9;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `bots_petdata` SET `have_saddle` = '9' WHERE `id` = '" + petUser.PetData.PetId + "' LIMIT 1");
                    dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + item.Id + "' LIMIT 1");
                }

                //We only want to use this if we're successful. 
                room.GetRoomItemHandler().RemoveFurniture(session, item.Id);
            }
            else if (item.Data.InteractionType == InteractionType.HORSE_SADDLE_2)
            {
                petUser.PetData.Saddle = 10;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `bots_petdata` SET `have_saddle` = '10' WHERE `id` = '" + petUser.PetData.PetId + "' LIMIT 1");
                    dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + item.Id + "' LIMIT 1");
                }

                //We only want to use this if we're successful. 
                room.GetRoomItemHandler().RemoveFurniture(session, item.Id);
            }
            else if (item.Data.InteractionType == InteractionType.HORSE_HAIRSTYLE)
            {
                int parse = 100;
                string hairType = item.GetBaseItem().ItemName.Split('_')[2];

                parse = parse + int.Parse(hairType);

                petUser.PetData.PetHair = parse;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `bots_petdata` SET `pethair` = '" + petUser.PetData.PetHair + "' WHERE `id` = '" + petUser.PetData.PetId + "' LIMIT 1");
                    dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + item.Id + "' LIMIT 1");
                }

                //We only want to use this if we're successful. 
                room.GetRoomItemHandler().RemoveFurniture(session, item.Id);
            }
            else if (item.Data.InteractionType == InteractionType.HORSE_HAIR_DYE)
            {
                int hairDye = 48;
                string hairType = item.GetBaseItem().ItemName.Split('_')[2];
            
                hairDye = hairDye + int.Parse(hairType);
                petUser.PetData.HairDye = hairDye;

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `bots_petdata` SET `hairdye` = '" + petUser.PetData.HairDye + "' WHERE `id` = '" + petUser.PetData.PetId + "' LIMIT 1");
                    dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + item.Id + "' LIMIT 1");
                }

                //We only want to use this if we're successful. 
                room.GetRoomItemHandler().RemoveFurniture(session, item.Id);
            }
            else if (item.Data.InteractionType == InteractionType.HORSE_BODY_DYE)
            {
                string race = item.GetBaseItem().ItemName.Split('_')[2];
                int parse = int.Parse(race);
                int raceLast = 2 + (parse * 4) - 4;
                if (parse == 13)
                    raceLast = 61;
                else if (parse == 14)
                    raceLast = 65;
                else if (parse == 15)
                    raceLast = 69;
                else if (parse == 16)
                    raceLast = 73;
                petUser.PetData.Race = raceLast.ToString();

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `bots_petdata` SET `race` = '" + petUser.PetData.Race + "' WHERE `id` = '" + petUser.PetData.PetId + "' LIMIT 1");
                    dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + item.Id + "' LIMIT 1");
                }

                //We only want to use this if we're successful. 
                room.GetRoomItemHandler().RemoveFurniture(session, item.Id);
            }

            //Update the Pet and the Pet figure information.
            room.SendPacket(new UsersComposer(petUser));
            room.SendPacket(new PetHorseFigureInformationComposer(petUser));
        }
    }
}