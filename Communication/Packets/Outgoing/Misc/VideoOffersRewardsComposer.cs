namespace Plus.Communication.Packets.Outgoing.Misc
{
    class VideoOffersRewardsComposer : MessageComposer
    {
        public int OfferId { get; }
        public string Type { get; }
        public string Message { get; }

        public VideoOffersRewardsComposer(int Id, string Type, string Message)
            : base(ServerPacketHeader.VideoOffersRewardsMessageComposer)
        {
            this.OfferId = Id;
            this.Type = Type;
            this.Message = Message;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(Type);
            packet.WriteInteger(OfferId);
            packet.WriteString(Message);
            packet.WriteString("");
        }
    }
}

 