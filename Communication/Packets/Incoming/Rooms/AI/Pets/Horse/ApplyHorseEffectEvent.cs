using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;



using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.AI.Pets;

using Plus.Database.Interfaces;

namespace Plus.Communication.Packets.Incoming.Rooms.AI.Pets.Horse
{
    class ApplyHorseEffectEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            int ItemId = Packet.PopInt();
            Item Item = Room.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null)
                return;

            int PetId = Packet.PopInt();

            RoomUser PetUser = null;
            if (!Room.GetRoomUserManager().TryGetPet(PetId, out PetUser))
                return;

            if (PetUser.PetData == null || PetUser.PetData.OwnerId != Session.GetHabbo().Id)
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
                Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id, false);
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
                Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id, false);
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
                Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id, false);
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
                Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id, false);
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
                Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id, false);
            }

            //Update the Pet and the Pet figure information.
            Room.SendPacket(new UsersComposer(PetUser));
            Room.SendPacket(new PetHorseFigureInformationComposer(PetUser));
        }
    }
}