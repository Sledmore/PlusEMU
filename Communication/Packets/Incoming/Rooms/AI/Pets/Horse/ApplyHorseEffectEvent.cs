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

            int ItemId = packet.PopInt();
            Item Item = room.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null)
                return;

            int PetId = packet.PopInt();
            
            if (!room.GetRoomUserManager().TryGetPet(PetId, out RoomUser PetUser))
                return;

            if (PetUser.PetData == null || PetUser.PetData.OwnerId != session.GetHabbo().Id)
                return;

            if (Item.Data.InteractionType == InteractionType.HORSE_SADDLE_1)
            {
                PetUser.PetData.Saddle = 9;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `bots_petdata` SET `have_saddle` = '9' WHERE `id` = '" + PetUser.PetData.PetId + "' LIMIT 1");
                    dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + Item.Id + "' LIMIT 1");
                }

                //We only want to use this if we're successful. 
                room.GetRoomItemHandler().RemoveFurniture(session, Item.Id);
            }
            else if (Item.Data.InteractionType == InteractionType.HORSE_SADDLE_2)
            {
                PetUser.PetData.Saddle = 10;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `bots_petdata` SET `have_saddle` = '10' WHERE `id` = '" + PetUser.PetData.PetId + "' LIMIT 1");
                    dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + Item.Id + "' LIMIT 1");
                }

                //We only want to use this if we're successful. 
                room.GetRoomItemHandler().RemoveFurniture(session, Item.Id);
            }
            else if (Item.Data.InteractionType == InteractionType.HORSE_HAIRSTYLE)
            {
                int Parse = 100;
                string HairType = Item.GetBaseItem().ItemName.Split('_')[2];

                Parse = Parse + int.Parse(HairType);

                PetUser.PetData.PetHair = Parse;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `bots_petdata` SET `pethair` = '" + PetUser.PetData.PetHair + "' WHERE `id` = '" + PetUser.PetData.PetId + "' LIMIT 1");
                    dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + Item.Id + "' LIMIT 1");
                }

                //We only want to use this if we're successful. 
                room.GetRoomItemHandler().RemoveFurniture(session, Item.Id);
            }
            else if (Item.Data.InteractionType == InteractionType.HORSE_HAIR_DYE)
            {
                int HairDye = 48;
                string HairType = Item.GetBaseItem().ItemName.Split('_')[2];
            
                HairDye = HairDye + int.Parse(HairType);
                PetUser.PetData.HairDye = HairDye;

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `bots_petdata` SET `hairdye` = '" + PetUser.PetData.HairDye + "' WHERE `id` = '" + PetUser.PetData.PetId + "' LIMIT 1");
                    dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + Item.Id + "' LIMIT 1");
                }

                //We only want to use this if we're successful. 
                room.GetRoomItemHandler().RemoveFurniture(session, Item.Id);
            }
            else if (Item.Data.InteractionType == InteractionType.HORSE_BODY_DYE)
            {
                string Race = Item.GetBaseItem().ItemName.Split('_')[2];
                int Parse = int.Parse(Race);
                int RaceLast = 2 + (Parse * 4) - 4;
                if (Parse == 13)
                    RaceLast = 61;
                else if (Parse == 14)
                    RaceLast = 65;
                else if (Parse == 15)
                    RaceLast = 69;
                else if (Parse == 16)
                    RaceLast = 73;
                PetUser.PetData.Race = RaceLast.ToString();

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `bots_petdata` SET `race` = '" + PetUser.PetData.Race + "' WHERE `id` = '" + PetUser.PetData.PetId + "' LIMIT 1");
                    dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + Item.Id + "' LIMIT 1");
                }

                //We only want to use this if we're successful. 
                room.GetRoomItemHandler().RemoveFurniture(session, Item.Id);
            }

            //Update the Pet and the Pet figure information.
            room.SendPacket(new UsersComposer(PetUser));
            room.SendPacket(new PetHorseFigureInformationComposer(PetUser));
        }
    }
}