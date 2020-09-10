using System.Collections.Generic;

using Plus.HabboHotel.Users;
using Plus.HabboHotel.Rooms;
using Plus.Utilities;
using Plus.HabboHotel.Rooms.Chat.Logs;

namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class ModeratorUserChatlogComposer : MessageComposer
    {
        public Habbo Habbo { get; }
        public List<KeyValuePair<RoomData, List<ChatlogEntry>>> ChatLogs { get; }

        public ModeratorUserChatlogComposer(Habbo habbo, List<KeyValuePair<RoomData, List<ChatlogEntry>>> chatlogs)
            : base(ServerPacketHeader.ModeratorUserChatlogMessageComposer)
        {
            this.Habbo = habbo;
            this.ChatLogs = chatlogs;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Habbo.Id);
            packet.WriteString(Habbo.Username);

            packet.WriteInteger(ChatLogs.Count); // Room Visits Count
            foreach (KeyValuePair<RoomData, List<ChatlogEntry>> Chatlog in ChatLogs)
            {
                packet.WriteByte(1);
                packet.WriteShort(2);//Count
                packet.WriteString("roomName");
                packet.WriteByte(2);
                packet.WriteString(Chatlog.Key.Name); // room name
                packet.WriteString("roomId");
                packet.WriteByte(1);
                packet.WriteInteger(Chatlog.Key.Id);

                packet.WriteShort(Chatlog.Value.Count); // Chatlogs Count
                foreach (ChatlogEntry Entry in Chatlog.Value)
                {
                    string Username = "NOT FOUND";
                    if (Entry.PlayerNullable() != null)
                    {
                        Username = Entry.PlayerNullable().Username;
                    }

                    packet.WriteString(UnixTimestamp.FromUnixTimestamp(Entry.Timestamp).ToShortTimeString());
                    packet.WriteInteger(Entry.PlayerId); // UserId of message
                    packet.WriteString(Username); // Username of message
                    packet.WriteString(!string.IsNullOrEmpty(Entry.Message) ? Entry.Message : "** user sent a blank message **"); // Message        
                    packet.WriteBoolean(Habbo.Id == Entry.PlayerId);
                }
            }
        }
    }
}