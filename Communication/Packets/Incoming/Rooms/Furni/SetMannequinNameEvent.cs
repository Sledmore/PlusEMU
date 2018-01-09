using System;
using System.Linq;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;


namespace Plus.Communication.Packets.Incoming.Rooms.Furni
{
    class SetMannequinNameEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            Room room = session.GetHabbo().CurrentRoom;
            if (room == null || !room.CheckRights(session, true))
                return;

            int itemId = packet.PopInt();
            string name = packet.PopString();

            Item item = session.GetHabbo().CurrentRoom.GetRoomItemHandler().GetItem(itemId);
            if (item == null)
                return;

            if (item.ExtraData.Contains(Convert.ToChar(5)))
            {
                string[] flags = item.ExtraData.Split(Convert.ToChar(5));
                item.ExtraData = flags[0] + Convert.ToChar(5) + flags[1] + Convert.ToChar(5) + name;
            }
            else
                item.ExtraData = "m" + Convert.ToChar(5) + ".ch-210-1321.lg-285-92" + Convert.ToChar(5) + "Default Mannequin";

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `items` SET `extra_data` = @Ed WHERE `id` = @itemId LIMIT 1");
                dbClient.AddParameter("itemId", item.Id);
                dbClient.AddParameter("Ed", item.ExtraData);
                dbClient.RunQuery();
            }

            item.UpdateState(true, true);
        }
    }
}
