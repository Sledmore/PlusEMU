using System;
using System.Data;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.Database.Interfaces;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class GetModeratorUserRoomVisitsEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            int userId = packet.PopInt();
            GameClient target = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(userId);
            if (target == null)
                return;

            Dictionary<double, RoomData> visits = new Dictionary<double, RoomData>();
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `room_id`, `entry_timestamp` FROM `user_roomvisits` WHERE `user_id` = @id ORDER BY `entry_timestamp` DESC LIMIT 50");
                dbClient.AddParameter("id", userId);
                DataTable table = dbClient.GetTable();

                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        if (!RoomFactory.TryGetData(Convert.ToInt32(row["room_id"]), out RoomData data))
                            continue;

                        if (!visits.ContainsKey(Convert.ToDouble(row["entry_timestamp"])))
                            visits.Add(Convert.ToDouble(row["entry_timestamp"]), data);
                    }
                }
            }

            session.SendPacket(new ModeratorUserRoomVisitsComposer(target.GetHabbo(), visits));
        }
    }
}