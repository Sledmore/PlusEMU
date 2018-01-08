namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class ModeratorSupportTicketResponseComposer : ServerPacket
    {
        public ModeratorSupportTicketResponseComposer(int result)
            : base(ServerPacketHeader.ModeratorSupportTicketResponseMessageComposer)
        {
            WriteInteger(result);
            WriteString("");
        }
    }
}