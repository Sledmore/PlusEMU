using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Moderation;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class CallForHelpPendingCallsDeletedEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null)
                return;
            
            if (PlusEnvironment.GetGame().GetModerationManager().UserHasTickets(session.GetHabbo().Id))
            {
                ModerationTicket PendingTicket = PlusEnvironment.GetGame().GetModerationManager().GetTicketBySenderId(session.GetHabbo().Id);
                if (PendingTicket != null)
                {
                    PendingTicket.Answered = true;
                    PlusEnvironment.GetGame().GetClientManager().SendPacket(new ModeratorSupportTicketComposer(session.GetHabbo().Id, PendingTicket), "mod_tool");
                }
            }
        }
    }
}
