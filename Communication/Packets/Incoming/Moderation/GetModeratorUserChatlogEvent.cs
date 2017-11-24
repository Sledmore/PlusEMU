using System;
using System.Data;
using System.Collections.Generic;

using Plus.HabboHotel.Users;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.Rooms.Chat.Logs;
using Plus.Database.Interfaces;
using Plus.Utilities;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class GetModeratorUserChatlogEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            Habbo Data = PlusEnvironment.GetHabboById(Packet.PopInt());
            if (Data == null)
            {
                Session.SendNotification("Unable to load info for user.");
                return;
            }

            PlusEnvironment.GetGame().GetChatManager().GetLogs().FlushAndSave();

            List<KeyValuePair<RoomData, List<ChatlogEntry>>> Chatlogs = new List<KeyValuePair<RoomData, List<ChatlogEntry>>>();
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `room_id`,`entry_timestamp`,`exit_timestamp` FROM `user_roomvisits` WHERE `user_id` = '" + Data.Id + "' ORDER BY `entry_timestamp` DESC LIMIT 7");
                DataTable GetLogs = dbClient.GetTable();

                if (GetLogs != null)
                {
                    foreach (DataRow Row in GetLogs.Rows)
                    {
                        RoomData RoomData = PlusEnvironment.GetGame().GetRoomManager().GenerateRoomData(Convert.ToInt32(Row["room_id"]));
                        if (RoomData == null)
                        {
                            continue;
                        }

                        double TimestampExit = (Convert.ToDouble(Row["exit_timestamp"]) <= 0 ? UnixTimestamp.GetNow() : Convert.ToDouble(Row["exit_timestamp"]));

                        Chatlogs.Add(new KeyValuePair<RoomData, List<ChatlogEntry>>(RoomData, GetChatlogs(RoomData, Convert.ToDouble(Row["entry_timestamp"]), TimestampExit)));
                    }
                }

                Session.SendPacket(new ModeratorUserChatlogComposer(Data, Chatlogs));
            }
        }

        private List<ChatlogEntry> GetChatlogs(RoomData RoomData, double TimeEnter, double TimeExit)
        {
            List<ChatlogEntry> Chats = new List<ChatlogEntry>();

            DataTable Data = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `user_id`, `timestamp`, `message` FROM `chatlogs` WHERE `room_id` = " + RoomData.Id + " AND `timestamp` > " + TimeEnter + " AND `timestamp` < " + TimeExit + " ORDER BY `timestamp` DESC LIMIT 100");
                Data = dbClient.GetTable();

                if (Data != null)
                {
                    foreach (DataRow Row in Data.Rows)
                    {
                        Habbo Habbo = PlusEnvironment.GetHabboById(Convert.ToInt32(Row["user_id"]));

                        if (Habbo != null)
                        {
                            Chats.Add(new ChatlogEntry(Convert.ToInt32(Row["user_id"]), RoomData.Id, Convert.ToString(Row["message"]), Convert.ToDouble(Row["timestamp"]), Habbo));
                        }
                    }
                }
            }

            return Chats;
        }
    }
}
