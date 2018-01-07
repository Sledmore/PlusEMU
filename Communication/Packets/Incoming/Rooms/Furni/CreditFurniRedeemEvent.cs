using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.Communication.Packets.Outgoing.Inventory.Furni;

using Plus.Database.Interfaces;


namespace Plus.Communication.Packets.Incoming.Rooms.Furni
{
    class CreditFurniRedeemEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;
            
            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
                return;

            if (!Room.CheckRights(Session, true))
                return;
            
            if (PlusEnvironment.GetSettingsManager().TryGetValue("room.item.exchangeables.enabled") != "1")
            {
                Session.SendNotification("The hotel managers have temporarilly disabled exchanging!");
                return;
            }

            Item Exchange = Room.GetRoomItemHandler().GetItem(Packet.PopInt());
            if (Exchange == null)
                return;

            if (Exchange.Data.InteractionType != InteractionType.EXCHANGE)
                return;


            int Value = Exchange.Data.BehaviourData;

            if (Value > 0)
            {
                Session.GetHabbo().Credits += Value;
                Session.SendPacket(new CreditBalanceComposer(Session.GetHabbo().Credits));
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM `items` WHERE `id` = @exchangeId LIMIT 1");
                dbClient.AddParameter("exchangeId", Exchange.Id);
                dbClient.RunQuery();
            }

            Session.SendPacket(new FurniListUpdateComposer());
            Room.GetRoomItemHandler().RemoveFurniture(null, Exchange.Id);
            Session.GetHabbo().GetInventoryComponent().RemoveItem(Exchange.Id);

        }
    }
}
