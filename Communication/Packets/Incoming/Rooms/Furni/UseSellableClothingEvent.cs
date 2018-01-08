using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Catalog.Clothing;



using Plus.Communication.Packets.Outgoing.Rooms.Notifications;
using Plus.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni
{
    class UseSellableClothingEvent : IPacketEvent
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
            if (item == null)
                return;

            if (item.Data == null)
                return;

            if (item.UserID != session.GetHabbo().Id)
                return;

            if (item.Data.InteractionType != InteractionType.PURCHASABLE_CLOTHING)
            {
                session.SendNotification("Oops, this item isn't set as a sellable clothing item!");
                return;
            }

            if (item.Data.BehaviourData == 0)
            {
                session.SendNotification("Oops, this item doesn't have a linking clothing configuration, please report it!");
                return;
            }

            if (!PlusEnvironment.GetGame().GetCatalog().GetClothingManager().TryGetClothing(item.Data.BehaviourData, out ClothingItem clothing))
            {
                session.SendNotification("Oops, we couldn't find this clothing part!");
                return;
            }

            //Quickly delete it from the database.
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM `items` WHERE `id` = @ItemId LIMIT 1");
                dbClient.AddParameter("ItemId", item.Id);
                dbClient.RunQuery();
            }

            //Remove the item.
            room.GetRoomItemHandler().RemoveFurniture(session, item.Id);

            session.GetHabbo().GetClothing().AddClothing(clothing.ClothingName, clothing.PartIds);
            session.SendPacket(new FigureSetIdsComposer(session.GetHabbo().GetClothing().GetClothingParts));
            session.SendPacket(new RoomNotificationComposer("figureset.redeemed.success"));
            session.SendWhisper("If for some reason cannot see your new clothing, reload the hotel!");
        }
    }
}
