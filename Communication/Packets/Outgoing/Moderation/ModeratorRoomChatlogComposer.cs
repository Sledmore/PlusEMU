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
            base.WriteByte(1);
            base.WriteShort(2);//Count
            base.WriteString("roomName");
            base.WriteByte(2);
            base.WriteString(room.Name);
            base.WriteString("roomId");
            base.WriteByte(1);
            base.WriteInteger(room.Id);

            base.WriteShort(chats.Count);
            foreach (ChatlogEntry Entry in chats)
            {
                string Username = "Unknown";
                if (Entry.PlayerNullable() != null)
                {
                    Username = Entry.PlayerNullable().Username;
                }

                base.WriteString(UnixTimestamp.FromUnixTimestamp(Entry.Timestamp).ToShortTimeString()); // time?
                base.WriteInteger(Entry.PlayerId); // User Id
                base.WriteString(Username); // Username
                base.WriteString(!string.IsNullOrEmpty(Entry.Message) ? Entry.Message : "** user sent a blank message **"); // Message        
                base.WriteBoolean(false); //TODO, AI's?
            }
        }
    }
}
