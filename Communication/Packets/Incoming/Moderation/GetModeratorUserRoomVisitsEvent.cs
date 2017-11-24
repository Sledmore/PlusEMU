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
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            int UserId = Packet.PopInt();
            GameClient Target = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
            if (Target == null)
                return;

            DataTable Table = null;
            Dictionary<double, RoomData> Visits = new Dictionary<double, RoomData>();
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `room_id`, `entry_timestamp` FROM `user_roomvisits` WHERE `user_id` = @id ORDER BY `entry_timestamp` DESC LIMIT 50");
                dbClient.AddParameter("id", UserId);
                Table = dbClient.GetTable();

                if (Table != null)
                {
                    foreach (DataRow Row in Table.Rows)
                    {
                        RoomData RData = PlusEnvironment.GetGame().GetRoomManager().GenerateRoomData(Convert.ToInt32(Row["room_id"]));
                        if (RData == null)
                            return;

                        if (!Visits.ContainsKey(Convert.ToDouble(Row["entry_timestamp"])))
                            Visits.Add(Convert.ToDouble(Row["entry_timestamp"]), RData);
                    }
                }
            }

            Session.SendPacket(new ModeratorUserRoomVisitsComposer(Target.GetHabbo(), Visits));
        }
    }
}