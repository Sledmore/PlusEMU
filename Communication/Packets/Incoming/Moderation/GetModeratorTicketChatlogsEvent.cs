using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Moderation;
using Plus.Communication.Packets.Outgoing.Moderation;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class GetModeratorTicketChatlogsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().GetPermissions().HasRight("mod_tickets"))
                return;

            int TicketId = Packet.PopInt();

            ModerationTicket Ticket = null;
            if (!PlusEnvironment.GetGame().GetModerationManager().TryGetTicket(TicketId, out Ticket) || Ticket.Room == null)
                return;

            RoomData Data = PlusEnvironment.GetGame().GetRoomManager().GenerateRoomData(Ticket.Room.Id);
            if (Data == null)
                return;

            Session.SendPacket(new ModeratorTicketChatlogComposer(Ticket, Data, Ticket.Timestamp));
        }
    }
}
