using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Catalog.Clothing;



using Plus.Communication.Packets.Outgoing.Rooms.Notifications;
using Plus.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Plus.Database.Interfaces;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni
{
    class UseSellableClothingEvent : IPacketEvent
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
            if (Item == null)
                return;

            if (Item.Data == null)
                return;

            if (Item.UserID != Session.GetHabbo().Id)
                return;

            if (Item.Data.InteractionType != InteractionType.PURCHASABLE_CLOTHING)
            {
                Session.SendNotification("Oops, this item isn't set as a sellable clothing item!");
                return;
            }

            if (Item.Data.BehaviourData == 0)
            {
                Session.SendNotification("Oops, this item doesn't have a linking clothing configuration, please report it!");
                return;
            }

            ClothingItem Clothing = null;
            if (!PlusEnvironment.GetGame().GetCatalog().GetClothingManager().TryGetClothing(Item.Data.BehaviourData, out Clothing))
            {
                Session.SendNotification("Oops, we couldn't find this clothing part!");
                return;
            }

            //Quickly delete it from the database.
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM `items` WHERE `id` = @ItemId LIMIT 1");
                dbClient.AddParameter("ItemId", Item.Id);
                dbClient.RunQuery();
            }

            //Remove the item.
            Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);

            Session.GetHabbo().GetClothing().AddClothing(Clothing.ClothingName, Clothing.PartIds);
            Session.SendPacket(new FigureSetIdsComposer(Session.GetHabbo().GetClothing().GetClothingParts));
            Session.SendPacket(new RoomNotificationComposer("figureset.redeemed.success"));
            Session.SendWhisper("If for some reason cannot see your new clothing, reload the hotel!");
        }
    }
}
