using System.Collections.Generic;
using Plus.HabboHotel.Rooms;

using Plus.Utilities;
using Plus.HabboHotel.Rooms.Chat.Logs;

namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class ModeratorRoomChatlogComposer : MessageComposer
    {
        public Room Room { get; }
        public ICollection<ChatlogEntry> Chats { get; }

        public ModeratorRoomChatlogComposer(Room room, ICollection<ChatlogEntry> chats)
            : base(ServerPacketHeader.ModeratorRoomChatlogMessageComposer)
        {
            this.Room = room;
            this.Chats = chats;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteByte(1);
            packet.WriteShort(2);//Count
            packet.WriteString("roomName");
            packet.WriteByte(2);
            packet.WriteString(Room.Name);
            packet.WriteString("roomId");
            packet.WriteByte(1);
            packet.WriteInteger(Room.Id);

            packet.WriteShort(Chats.Count);
            foreach (ChatlogEntry Entry in Chats)
            {
                string Username = "Unknown";
                if (Entry.PlayerNullable() != null)
                {
                    Username = Entry.PlayerNullable().Username;
                }

                packet.WriteString(UnixTimestamp.FromUnixTimestamp(Entry.Timestamp).ToShortTimeString()); // time?
                packet.WriteInteger(Entry.PlayerId); // User Id
                packet.WriteString(Username); // Username
                packet.WriteString(!string.IsNullOrEmpty(Entry.Message) ? Entry.Message : "** user sent a blank message **"); // Message        
                packet.WriteBoolean(false); //TODO, AI's?
            }
        }
    }
}
