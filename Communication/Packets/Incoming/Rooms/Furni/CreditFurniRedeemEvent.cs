using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.Communication.Packets.Outgoing.Inventory.Furni;

using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;


namespace Plus.Communication.Packets.Incoming.Rooms.Furni
{
    class CreditFurniRedeemEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;
            
            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(session, true))
                return;
            
            if (PlusEnvironment.GetSettingsManager().TryGetValue("room.item.exchangeables.enabled") != "1")
            {
                session.SendNotification("The hotel managers have temporarilly disabled exchanging!");
                return;
            }

            Item exchange = room.GetRoomItemHandler().GetItem(packet.PopInt());
            if (exchange == null)
                return;

            if (exchange.Data.InteractionType != InteractionType.EXCHANGE)
                return;


            int value = exchange.Data.BehaviourData;

            if (value > 0)
            {
                session.GetHabbo().Credits += value;
                session.SendPacket(new CreditBalanceComposer(session.GetHabbo().Credits));
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM `items` WHERE `id` = @exchangeId LIMIT 1");
                dbClient.AddParameter("exchangeId", exchange.Id);
                dbClient.RunQuery();
            }

            session.SendPacket(new FurniListUpdateComposer());
            room.GetRoomItemHandler().RemoveFurniture(null, exchange.Id);
            session.GetHabbo().GetInventoryComponent().RemoveItem(exchange.Id);

        }
    }
}
