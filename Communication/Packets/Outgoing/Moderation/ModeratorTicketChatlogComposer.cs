using Plus.Utilities;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Moderation;

namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class ModeratorTicketChatlogComposer : MessageComposer
    {
        public ModerationTicket ModerationTicket { get; }
        public RoomData RoomData { get; }
        public double Timestamp { get; }

        public ModeratorTicketChatlogComposer(ModerationTicket ticket, RoomData roomData, double timestamp)
              : base(ServerPacketHeader.ModeratorTicketChatlogMessageComposer)
        {
            this.ModerationTicket = ticket;
            this.RoomData = roomData;
            this.Timestamp = timestamp;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(ModerationTicket.Id);
            packet.WriteInteger(ModerationTicket.Sender != null ? ModerationTicket.Sender.Id : 0);
            packet.WriteInteger(ModerationTicket.Reported != null ? ModerationTicket.Reported.Id : 0);
            packet.WriteInteger(RoomData.Id);

            packet.WriteByte(1);
            packet.WriteShort(2);//Count
            packet.WriteString("roomName");
            packet.WriteByte(2);
            packet.WriteString(RoomData.Name);
            packet.WriteString("roomId");
            packet.WriteByte(1);
            packet.WriteInteger(RoomData.Id);

            packet.WriteShort(ModerationTicket.ReportedChats.Count);
            foreach (string Chat in ModerationTicket.ReportedChats)
            {
                packet.WriteString(UnixTimestamp.FromUnixTimestamp(Timestamp).ToShortTimeString());
                packet.WriteInteger(ModerationTicket.Id);
                packet.WriteString(ModerationTicket.Reported != null ? ModerationTicket.Reported.Username : "No username");
                packet.WriteString(Chat);
                packet.WriteBoolean(false);
            }
        }
    }
}
