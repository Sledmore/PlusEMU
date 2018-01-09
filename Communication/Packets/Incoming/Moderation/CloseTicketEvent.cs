using Plus.HabboHotel.Moderation;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;
using Plus.Database.Interfaces;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class CloseTicketEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || !session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            int result = packet.PopInt(); // 1 = useless, 2 = abusive, 3 = resolved
            packet.PopInt(); //junk
            int ticketId = packet.PopInt();
            
            if (!PlusEnvironment.GetGame().GetModerationManager().TryGetTicket(ticketId, out ModerationTicket ticket))
                return;

            if (ticket.Moderator.Id != session.GetHabbo().Id)
                return;

            GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(ticket.Sender.Id);
            if (client != null)
            {
                client.SendPacket(new ModeratorSupportTicketResponseComposer(result));
            }

            if (result == 2)
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `user_info` SET `cfhs_abusive` = `cfhs_abusive` + 1 WHERE `user_id` = '" + ticket.Sender.Id + "' LIMIT 1");
                }
            }

            ticket.Answered = true;
            PlusEnvironment.GetGame().GetClientManager().SendPacket(new ModeratorSupportTicketComposer(session.GetHabbo().Id, ticket), "mod_tool");
        }
    }
}