using System;
using System.Data;
using System.Collections.Generic;

using Plus.HabboHotel.Users;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.Rooms.Chat.Logs;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.Utilities;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class GetModeratorUserChatlogEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null)
                return;

            if (!session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            Habbo data = PlusEnvironment.GetHabboById(packet.PopInt());
            if (data == null)
            {
                session.SendNotification("Unable to load info for user.");
                return;
            }

            PlusEnvironment.GetGame().GetChatManager().GetLogs().FlushAndSave();

            List<KeyValuePair<RoomData, List<ChatlogEntry>>> chatlogs = new List<KeyValuePair<RoomData, List<ChatlogEntry>>>();
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `room_id`,`entry_timestamp`,`exit_timestamp` FROM `user_roomvisits` WHERE `user_id` = '" + data.Id + "' ORDER BY `entry_timestamp` DESC LIMIT 7");
                DataTable getLogs = dbClient.GetTable();

                if (getLogs != null)
                {
                    foreach (DataRow row in getLogs.Rows)
                    {
                        if (!RoomFactory.TryGetData(Convert.ToInt32(row["room_id"]), out RoomData roomData))
                            continue;

                        double timestampExit = (Convert.ToDouble(row["exit_timestamp"]) <= 0 ? UnixTimestamp.GetNow() : Convert.ToDouble(row["exit_timestamp"]));

                        chatlogs.Add(new KeyValuePair<RoomData, List<ChatlogEntry>>(roomData, GetChatlogs(roomData, Convert.ToDouble(row["entry_timestamp"]), timestampExit)));
                    }
                }

                session.SendPacket(new ModeratorUserChatlogComposer(data, chatlogs));
            }
        }

        private List<ChatlogEntry> GetChatlogs(RoomData roomData, double timeEnter, double timeExit)
        {
            List<ChatlogEntry> chats = new List<ChatlogEntry>();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `user_id`, `timestamp`, `message` FROM `chatlogs` WHERE `room_id` = " + roomData.Id + " AND `timestamp` > " + timeEnter + " AND `timestamp` < " + timeExit + " ORDER BY `timestamp` DESC LIMIT 100");
                var data = dbClient.GetTable();

                if (data != null)
                {
                    foreach (DataRow row in data.Rows)
                    {
                        Habbo habbo = PlusEnvironment.GetHabboById(Convert.ToInt32(row["user_id"]));

                        if (habbo != null)
                        {
                            chats.Add(new ChatlogEntry(Convert.ToInt32(row["user_id"]), roomData.Id, Convert.ToString(row["message"]), Convert.ToDouble(row["timestamp"]), habbo));
                        }
                    }
                }
            }

            return chats;
        }
    }
}
