using System.Collections.Generic;

using Plus.Utilities;
using Plus.HabboHotel.Users;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Moderation;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class SubmitNewTicketEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null)
                return;

            // Run a quick check to see if we have any existing tickets.
            if (PlusEnvironment.GetGame().GetModerationManager().UserHasTickets(session.GetHabbo().Id))
            {
                ModerationTicket pendingTicket = PlusEnvironment.GetGame().GetModerationManager().GetTicketBySenderId(session.GetHabbo().Id);
                if (pendingTicket != null)
                {
                    session.SendPacket(new CallForHelpPendingCallsComposer(pendingTicket));
                    return;
                }
            }

            List<string> chats = new List<string>();

            string message = StringCharFilter.Escape(packet.PopString().Trim());
            int category = packet.PopInt();
            int reportedUserId = packet.PopInt();
            int type = packet.PopInt();// Unsure on what this actually is.

            Habbo reportedUser = PlusEnvironment.GetHabboById(reportedUserId);
            if (reportedUser == null)
            {
                // User doesn't exist.
                return;
            }

            int messagecount = packet.PopInt();
            for (int i = 0; i < messagecount; i++)
            {
                packet.PopInt();
                chats.Add(packet.PopString());
            }

            ModerationTicket ticket = new ModerationTicket(1, type, category, UnixTimestamp.GetNow(), 1, session.GetHabbo(), reportedUser, message, session.GetHabbo().CurrentRoom, chats);
            if (!PlusEnvironment.GetGame().GetModerationManager().TryAddTicket(ticket))
                return;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                // TODO: Come back to this.
                /*dbClient.SetQuery("INSERT INTO `moderation_tickets` (`score`,`type`,`status`,`sender_id`,`reported_id`,`moderator_id`,`message`,`room_id`,`room_name`,`timestamp`) VALUES (1, '" + Category + "', 'open', '" + Session.GetHabbo().Id + "', '" + ReportedUserId + "', '0', @message, '0', '', '" + PlusEnvironment.GetUnixTimestamp() + "')");
                dbClient.AddParameter("message", Message);
                dbClient.RunQuery();*/

                dbClient.RunQuery("UPDATE `user_info` SET `cfhs` = `cfhs` + '1' WHERE `user_id` = '" + session.GetHabbo().Id + "' LIMIT 1");
            }

            PlusEnvironment.GetGame().GetClientManager().ModAlert("A new support ticket has been submitted!");
            PlusEnvironment.GetGame().GetClientManager().SendPacket(new ModeratorSupportTicketComposer(session.GetHabbo().Id, ticket), "mod_tool");
        }
    }
}
