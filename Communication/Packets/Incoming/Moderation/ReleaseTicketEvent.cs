using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.Moderation;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class ReleaseTicketEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            int Amount = Packet.PopInt();

            for (int i = 0; i < Amount; i++)
            {
                ModerationTicket Ticket = null;
                if (!PlusEnvironment.GetGame().GetModerationManager().TryGetTicket(Packet.PopInt(), out Ticket))
                    continue;

                Ticket.Moderator = null;
                PlusEnvironment.GetGame().GetClientManager().SendPacket(new ModeratorSupportTicketComposer(Session.GetHabbo().Id, Ticket), "mod_tool");
            }
        }
    }
}