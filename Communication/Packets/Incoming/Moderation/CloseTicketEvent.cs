using Plus.HabboHotel.Moderation;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;
using Plus.Database.Interfaces;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class CloseTicketEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || !session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            int Result = packet.PopInt(); // 1 = useless, 2 = abusive, 3 = resolved
            int Junk = packet.PopInt();
            int TicketId = packet.PopInt();
            
            if (!PlusEnvironment.GetGame().GetModerationManager().TryGetTicket(TicketId, out ModerationTicket ticket))
                return;

            if (ticket.Moderator.Id != session.GetHabbo().Id)
                return;

            GameClient Client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(ticket.Sender.Id);
            if (Client != null)
            {
                Client.SendPacket(new ModeratorSupportTicketResponseComposer(Result));
            }

            if (Result == 2)
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