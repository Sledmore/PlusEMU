namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class ModeratorSupportTicketResponseComposer : MessageComposer
    {
        public int Result { get; }

        public ModeratorSupportTicketResponseComposer(int result)
            : base(ServerPacketHeader.ModeratorSupportTicketResponseMessageComposer)
        {
            this.Result = result;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Result);
            packet.WriteString("");
        }
    }
}