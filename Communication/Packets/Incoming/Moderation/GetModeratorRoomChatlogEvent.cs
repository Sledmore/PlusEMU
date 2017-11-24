using System;
using System.Data;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Rooms.Chat.Logs;
using Plus.Communication.Packets.Outgoing.Moderation;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class GetModeratorRoomChatlogEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            int Junk = Packet.PopInt();
            int RoomId = Packet.PopInt();

            Room Room = null;
            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(RoomId, out Room))
            {
                return;
            }

            PlusEnvironment.GetGame().GetChatManager().GetLogs().FlushAndSave();

            List<ChatlogEntry> Chats = new List<ChatlogEntry>();

            DataTable Data = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `chatlogs` WHERE `room_id` = @id ORDER BY `id` DESC LIMIT 100");
                dbClient.AddParameter("id", RoomId);
                Data = dbClient.GetTable();

                if (Data != null)
                {
                    foreach (DataRow Row in Data.Rows)
                    {
                        Habbo Habbo = PlusEnvironment.GetHabboById(Convert.ToInt32(Row["user_id"]));

                        if (Habbo != null)
                        {
                            Chats.Add(new ChatlogEntry(Convert.ToInt32(Row["user_id"]), RoomId, Convert.ToString(Row["message"]), Convert.ToDouble(Row["timestamp"]), Habbo));
                        }
                    }
                }
            }

            Session.SendPacket(new ModeratorRoomChatlogComposer(Room, Chats));
        }
    }
}