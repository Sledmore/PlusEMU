using Plus.Utilities;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Moderation;

namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class ModeratorTicketChatlogComposer : ServerPacket
    {
        public ModeratorTicketChatlogComposer(ModerationTicket ticket, RoomData roomData, double timestamp)
              : base(ServerPacketHeader.ModeratorTicketChatlogMessageComposer)
        {
            base.WriteInteger(ticket.Id);
            base.WriteInteger(ticket.Sender != null ? ticket.Sender.Id : 0);
            base.WriteInteger(ticket.Reported != null ? ticket.Reported.Id : 0);
            base.WriteInteger(roomData.Id);

            base.WriteByte(1);
            base.WriteShort(2);//Count
            base.WriteString("roomName");
            base.WriteByte(2);
            base.WriteString(roomData.Name);
            base.WriteString("roomId");
            base.WriteByte(1);
            base.WriteInteger(roomData.Id);

            base.WriteShort(ticket.ReportedChats.Count);
            foreach (string Chat in ticket.ReportedChats)
            {
                base.WriteString(UnixTimestamp.FromUnixTimestamp(timestamp).ToShortTimeString());
                base.WriteInteger(ticket.Id);
                base.WriteString(ticket.Reported != null ? ticket.Reported.Username : "No username");
                base.WriteString(Chat);
                base.WriteBoolean(false);
            }
        }
    }
}
