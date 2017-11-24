using System.Collections.Generic;

using Plus.HabboHotel.Users;
using Plus.HabboHotel.Rooms;
using Plus.Utilities;
using Plus.HabboHotel.Rooms.Chat.Logs;

namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class ModeratorUserChatlogComposer : ServerPacket
    {
        public ModeratorUserChatlogComposer(Habbo habbo, List<KeyValuePair<RoomData, List<ChatlogEntry>>> chatlogs)
            : base(ServerPacketHeader.ModeratorUserChatlogMessageComposer)
        {
            base.WriteInteger(habbo.Id);
            base.WriteString(habbo.Username);

            base.WriteInteger(chatlogs.Count); // Room Visits Count
            foreach (KeyValuePair<RoomData, List<ChatlogEntry>> Chatlog in chatlogs)
            {
                base.WriteByte(1);
                base.WriteShort(2);//Count
                base.WriteString("roomName");
                base.WriteByte(2);
                base.WriteString(Chatlog.Key.Name); // room name
                base.WriteString("roomId");
                base.WriteByte(1);
                base.WriteInteger(Chatlog.Key.Id);

                base.WriteShort(Chatlog.Value.Count); // Chatlogs Count
                foreach (ChatlogEntry Entry in Chatlog.Value)
                {
                    string Username = "NOT FOUND";
                    if (Entry.PlayerNullable() != null)
                    {
                        Username = Entry.PlayerNullable().Username;
                    }

                    base.WriteString(UnixTimestamp.FromUnixTimestamp(Entry.Timestamp).ToShortTimeString());
                    base.WriteInteger(Entry.PlayerId); // UserId of message
                    base.WriteString(Username); // Username of message
                    base.WriteString(!string.IsNullOrEmpty(Entry.Message) ? Entry.Message : "** user sent a blank message **"); // Message        
                    base.WriteBoolean(habbo.Id == Entry.PlayerId);
                }
            }
        }
    }
}