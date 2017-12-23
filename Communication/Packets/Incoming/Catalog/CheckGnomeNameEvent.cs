using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms.AI;
using Plus.HabboHotel.Rooms.AI.Speech;
using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.Communication.Packets.Outgoing.Inventory.Furni;

using Plus.Database.Interfaces;
using Plus.HabboHotel.Catalog.Utilities;
using Plus.HabboHotel.Rooms.AI.Responses;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    class CheckGnomeNameEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            int ItemId = Packet.PopInt();
            Item Item = Room.GetRoomItemHandler().GetItem(ItemId);

            if (Item == null || Item.Data == null || Item.UserID != Session.GetHabbo().Id || Item.Data.InteractionType != InteractionType.GNOME_BOX)
                return;

            string PetName = Packet.PopString();
            if (string.IsNullOrEmpty(PetName))
            {
                Session.SendPacket(new CheckGnomeNameComposer(PetName, 1));
                return;
            }

            if (!PlusEnvironment.IsValidAlphaNumeric(PetName))
            {
                Session.SendPacket(new CheckGnomeNameComposer(PetName, 1));
                return;
            }

            int X = Item.GetX;
            int Y = Item.GetY;

            //Quickly delete it from the database.
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM `items` WHERE `id` = @ItemId LIMIT 1");
                dbClient.AddParameter("ItemId", Item.Id);
                dbClient.RunQuery();
            }

            //Remove the item.
            Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);

            //Apparently we need this for success.
            Session.SendPacket(new CheckGnomeNameComposer(PetName, 0));

            //Create the pet here.
            Pet Pet = PetUtility.CreatePet(Session.GetHabbo().Id, PetName, 26, "30", "ffffff");
            if (Pet == null)
            {
                Session.SendNotification("Oops, an error occoured. Please report this!");
                return;
            }

            List<RandomSpeech> RndSpeechList = new List<RandomSpeech>();

            Pet.RoomId = Session.GetHabbo().CurrentRoomId;
            Pet.GnomeClothing = RandomClothing();

            //Update the pets gnome clothing.
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `bots_petdata` SET `gnome_clothing` = @GnomeClothing WHERE `id` = @PetId LIMIT 1");
                dbClient.AddParameter("GnomeClothing", Pet.GnomeClothing);
                dbClient.AddParameter("PetId", Pet.PetId);
                dbClient.RunQuery();
            }

            //Make a RoomUser of the pet.
            RoomUser PetUser = Room.GetRoomUserManager().DeployBot(new RoomBot(Pet.PetId, Pet.RoomId, "pet", "freeroam", Pet.Name, "", Pet.Look, X, Y, 0, 0, 0, 0, 0, 0, ref RndSpeechList, "", 0, Pet.OwnerId, false, 0, false, 0), Pet);

            //Give the food.
            if (PlusEnvironment.GetGame().GetItemManager().GetItem(320, out ItemData PetFood))
            {
                Item Food = ItemFactory.CreateSingleItemNullable(PetFood, Session.GetHabbo(), "", "");
                if (Food != null)
                {
                    Session.GetHabbo().GetInventoryComponent().TryAddItem(Food);
                    Session.SendPacket(new FurniListNotificationComposer(Food.Id, 1));
                }
            }
        }

        private static string RandomClothing()
        {
            Random Random = new Random();

            int RandomNumber = Random.Next(1, 6);
            switch (RandomNumber)
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