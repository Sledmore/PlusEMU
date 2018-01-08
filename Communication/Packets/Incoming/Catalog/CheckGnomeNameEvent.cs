using System;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms.AI;
using Plus.HabboHotel.Rooms.AI.Speech;
using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.Communication.Packets.Outgoing.Inventory.Furni;

using Plus.Database.Interfaces;
using Plus.HabboHotel.Catalog.Utilities;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    class CheckGnomeNameEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || !session.GetHabbo().InRoom)
                return;

            Room room = session.GetHabbo().CurrentRoom;
            if (room == null)
                return;

            int itemId = packet.PopInt();
            Item item = room.GetRoomItemHandler().GetItem(itemId);

            if (item == null || item.Data == null || item.UserID != session.GetHabbo().Id || item.Data.InteractionType != InteractionType.GNOME_BOX)
                return;

            string petName = packet.PopString();
            if (string.IsNullOrEmpty(petName))
            {
                session.SendPacket(new CheckGnomeNameComposer(petName, 1));
                return;
            }

            if (!PlusEnvironment.IsValidAlphaNumeric(petName))
            {
                session.SendPacket(new CheckGnomeNameComposer(petName, 1));
                return;
            }

            int x = item.GetX;
            int y = item.GetY;

            //Quickly delete it from the database.
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM `items` WHERE `id` = @ItemId LIMIT 1");
                dbClient.AddParameter("ItemId", item.Id);
                dbClient.RunQuery();
            }

            //Remove the item.
            room.GetRoomItemHandler().RemoveFurniture(session, item.Id);

            //Apparently we need this for success.
            session.SendPacket(new CheckGnomeNameComposer(petName, 0));

            //Create the pet here.
            Pet pet = PetUtility.CreatePet(session.GetHabbo().Id, petName, 26, "30", "ffffff");
            if (pet == null)
            {
                session.SendNotification("Oops, an error occoured. Please report this!");
                return;
            }

            List<RandomSpeech> rndSpeechList = new List<RandomSpeech>();

            pet.RoomId = session.GetHabbo().CurrentRoomId;
            pet.GnomeClothing = RandomClothing();

            //Update the pets gnome clothing.
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `bots_petdata` SET `gnome_clothing` = @GnomeClothing WHERE `id` = @PetId LIMIT 1");
                dbClient.AddParameter("GnomeClothing", pet.GnomeClothing);
                dbClient.AddParameter("PetId", pet.PetId);
                dbClient.RunQuery();
            }

            //Make a RoomUser of the pet.
            room.GetRoomUserManager().DeployBot(new RoomBot(pet.PetId, pet.RoomId, "pet", "freeroam", pet.Name, "", pet.Look, x, y, 0, 0, 0, 0, 0, 0, ref rndSpeechList, "", 0, pet.OwnerId, false, 0, false, 0), pet);

            //Give the food.
            if (PlusEnvironment.GetGame().GetItemManager().GetItem(320, out ItemData petFood))
            {
                Item food = ItemFactory.CreateSingleItemNullable(petFood, session.GetHabbo(), "", "");
                if (food != null)
                {
                    session.GetHabbo().GetInventoryComponent().TryAddItem(food);
                    session.SendPacket(new FurniListNotificationComposer(food.Id, 1));
                }
            }
        }

        private static string RandomClothing()
        {
            Random random = new Random();

            int randomNumber = random.Next(1, 6);
            switch (randomNumber)
            {
                default:
                case 1:
                    return "5 0 -1 0 4 402 5 3 301 4 1 101 2 2 201 3";
                case 2:
                    return "5 0 -1 0 1 102 13 3 301 4 4 401 5 2 201 3";
                case 3:
                    return "5 1 102 8 2 201 16 4 401 9 3 303 4 0 -1 6";
                case 4:
                    return "5 0 -1 0 3 303 4 4 401 5 1 101 2 2 201 3";
                case 5:
                    return "5 3 302 4 2 201 11 1 102 12 0 -1 28 4 401 24";
                case 6:
                    return "5 4 402 5 3 302 21 0 -1 7 1 101 12 2 201 17";
            }
        }
    }
}