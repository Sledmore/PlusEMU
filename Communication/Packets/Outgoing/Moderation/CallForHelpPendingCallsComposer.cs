using Plus.HabboHotel.Moderation;
using Plus.Utilities;

namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class CallForHelpPendingCallsComposer  :ServerPacket
    {
        public CallForHelpPendingCallsComposer(ModerationTicket ticket)
            : base(ServerPacketHeader.CallForHelpPendingCallsMessageComposer)
        {
            WriteInteger(1);// Count for whatever reason?
            {
                WriteString(ticket.Id.ToString());
                WriteString(UnixTimestamp.FromUnixTimestamp(ticket.Timestamp).ToShortTimeString());// "11-02-2017 04:07:05";
                WriteString(ticket.Issue);
            }
        }
    }
}
