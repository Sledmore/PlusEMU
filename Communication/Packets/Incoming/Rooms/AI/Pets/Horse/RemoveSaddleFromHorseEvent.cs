using Plus.HabboHotel.Rooms;

using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.AI.Pets;

using Plus.HabboHotel.Catalog.Utilities;
using Plus.HabboHotel.Items;
using Plus.Communication.Packets.Outgoing.Inventory.Furni;
using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.AI.Pets.Horse
{
    class RemoveSaddleFromHorseEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;

            if (!room.GetRoomUserManager().TryGetPet(packet.PopInt(), out RoomUser petUser))
                return;

            if (petUser.PetData == null || petUser.PetData.OwnerId != session.GetHabbo().Id)
                return;

            //Fetch the furniture Id for the pets current saddle.
            int saddleId = ItemUtility.GetSaddleId(petUser.PetData.Saddle);

            //Remove the saddle from the pet.
            petUser.PetData.Saddle = 0;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `bots_petdata` SET `have_saddle` = '0' WHERE `id` = '" + petUser.PetData.PetId + "' LIMIT 1");
            }

            //Give the saddle back to the user.
            if (!PlusEnvironment.GetGame().GetItemManager().GetItem(saddleId, out ItemData itemData))
                return;

            Item item = ItemFactory.CreateSingleItemNullable(itemData, session.GetHabbo(), "", "");
            if (item != null)
            {
                session.GetHabbo().GetInventoryComponent().TryAddItem(item);
                session.SendPacket(new FurniListNotificationComposer(item.Id, 1));
                session.SendPacket(new PurchaseOKComposer());
                session.SendPacket(new FurniListAddComposer(item));
                session.SendPacket(new FurniListUpdateComposer());
            }

            //Update the Pet and the Pet figure information.
            room.SendPacket(new UsersComposer(petUser));
            room.SendPacket(new PetHorseFigureInformationComposer(petUser));
        }
    }
}
