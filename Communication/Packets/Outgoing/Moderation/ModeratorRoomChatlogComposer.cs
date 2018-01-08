using System.Collections.Generic;
using Plus.HabboHotel.Rooms;

using Plus.Utilities;
using Plus.HabboHotel.Rooms.Chat.Logs;

namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class ModeratorRoomChatlogComposer : ServerPacket
    {
        public ModeratorRoomChatlogComposer(Room room, ICollection<ChatlogEntry> chats)
            : base(ServerPacketHeader.ModeratorRoomChatlogMessageComposer)
        {
            WriteByte(1);
            WriteShort(2);//Count
            WriteString("roomName");
            WriteByte(2);
            WriteString(room.Name);
            WriteString("roomId");
            WriteByte(1);
            WriteInteger(room.Id);

            WriteShort(chats.Count);
            foreach (ChatlogEntry Entry in chats)
            {
                string Username = "Unknown";
                if (Entry.PlayerNullable() != null)
                {
                    Username = Entry.PlayerNullable().Username;
                }

                WriteString(UnixTimestamp.FromUnixTimestamp(Entry.Timestamp).ToShortTimeString()); // time?
                WriteInteger(Entry.PlayerId); // User Id
                WriteString(Username); // Username
                WriteString(!string.IsNullOrEmpty(Entry.Message) ? Entry.Message : "** user sent a blank message **"); // Message        
                WriteBoolean(false); //TODO, AI's?
            }
        }
    }
}
