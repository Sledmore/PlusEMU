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
                ModerationTicket pendingTicket = PlusEnvironment.GetGame().GetModerationManager().GetTicketBySenderId(session.GetHabbo().Id);
                if (pendingTicket != null)
                {
                    pendingTicket.Answered = true;
                    PlusEnvironment.GetGame().GetClientManager().SendPacket(new ModeratorSupportTicketComposer(session.GetHabbo().Id, pendingTicket), "mod_tool");
                }
            }
        }
    }
}
