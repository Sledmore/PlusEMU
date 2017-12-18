using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Moderation;
using Plus.Communication.Packets.Outgoing.Moderation;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class GetModeratorTicketChatlogsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || !session.GetHabbo().GetPermissions().HasRight("mod_tickets"))
                return;

            int ticketId = packet.PopInt();

            ModerationTicket ticket = null;
            if (!PlusEnvironment.GetGame().GetModerationManager().TryGetTicket(ticketId, out ticket) || ticket.Room == null)
                return;

            RoomData data = null;
            if (!RoomFactory.TryGetData(ticket.Room.Id, out data))
                return;

            session.SendPacket(new ModeratorTicketChatlogComposer(ticket, data, ticket.Timestamp));
        }
    }
}
