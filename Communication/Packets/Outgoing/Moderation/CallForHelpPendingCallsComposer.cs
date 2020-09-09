using Plus.HabboHotel.Moderation;
using Plus.Utilities;

namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class CallForHelpPendingCallsComposer : MessageComposer
    {
        public ModerationTicket Ticket { get; }

        public CallForHelpPendingCallsComposer(ModerationTicket ticket)
            : base(ServerPacketHeader.CallForHelpPendingCallsMessageComposer)
        {
            this.Ticket = ticket;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(1);// Count for whatever reason?
            {
                packet.WriteString(Ticket.Id.ToString());
                packet.WriteString(UnixTimestamp.FromUnixTimestamp(Ticket.Timestamp).ToShortTimeString());// "11-02-2017 04:07:05";
                packet.WriteString(Ticket.Issue);
            }
        }
    }
}
