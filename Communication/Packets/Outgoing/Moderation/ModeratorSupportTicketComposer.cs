using System;

using Plus.Utilities;
using Plus.HabboHotel.Moderation;

namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class ModeratorSupportTicketComposer : ServerPacket
    {
        public ModeratorSupportTicketComposer(int Id, ModerationTicket Ticket)
          : base(ServerPacketHeader.ModeratorSupportTicketMessageComposer)
        {
            base.WriteInteger(Ticket.Id); // Id
            base.WriteInteger(Ticket.GetStatus(Id)); // Tab ID
            base.WriteInteger(Ticket.Type); // Type
            base.WriteInteger(Ticket.Category); // Category 
            base.WriteInteger(Convert.ToInt32((DateTime.Now - UnixTimestamp.FromUnixTimestamp(Ticket.Timestamp)).TotalMilliseconds)); // This should fix the overflow?
            base.WriteInteger(Ticket.Priority); // Priority
            base.WriteInteger(0);//??
            base.WriteInteger(Ticket.Sender == null ? 0 : Ticket.Sender.Id); // Sender ID
                                                                             //base.WriteInteger(1);
            base.WriteString(Ticket.Sender == null ? string.Empty : Ticket.Sender.Username); // Sender Name
            base.WriteInteger(Ticket.Reported == null ? 0 : Ticket.Reported.Id); // Reported ID
            base.WriteString(Ticket.Reported == null ? string.Empty : Ticket.Reported.Username); // Reported Name
            base.WriteInteger(Ticket.Moderator == null ? 0 : Ticket.Moderator.Id); // Moderator ID
            base.WriteString(Ticket.Moderator == null ? string.Empty : Ticket.Moderator.Username); // Mod Name
            base.WriteString(Ticket.Issue); // Issue
            base.WriteInteger(Ticket.Room == null ? 0 : Ticket.Room.Id); // Room Id
            base.WriteInteger(0);
            {
                // push String
                // push Integer
                // push Integer
            }
        }

        /*public ModeratorSupportTicketComposer(SupportTicket Ticket)
             : base(ServerPacketHeader.ModeratorSupportTicketMessageComposer)
         {
             base.WriteInteger(Ticket.Id);
             base.WriteInteger(Ticket.TabId);
             base.WriteInteger(1); // Type
             base.WriteInteger(114); // Category
             base.WriteInteger(((int)PlusEnvironment.GetUnixTimestamp() - (int)Ticket.Timestamp) * 1000);
             base.WriteInteger(Ticket.Score);
             base.WriteInteger(0);
             base.WriteInteger(Ticket.SenderId);
             base.WriteString(Ticket.SenderName);
             base.WriteInteger(Ticket.ReportedId);
             base.WriteString(Ticket.ReportedName);
             base.WriteInteger((Ticket.Status == TicketStatus.PICKED) ? Ticket.ModeratorId : 0);
             base.WriteString(Ticket.ModName);
             base.WriteString(Ticket.Message);
             base.WriteInteger(0);//No idea?
             base.WriteInteger(0);//String, int, int - this is the "matched to" a string
             {
                 base.WriteString("fresh-hotel.org");
                 base.WriteInteger(-1);
                 base.WriteInteger(-1);
             }
         }*/
    }
}