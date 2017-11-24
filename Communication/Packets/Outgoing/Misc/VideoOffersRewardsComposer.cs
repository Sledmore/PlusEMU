namespace Plus.Communication.Packets.Outgoing.Misc
{
    class VideoOffersRewardsComposer : ServerPacket
    {
        public VideoOffersRewardsComposer(int Id, string Type, string Message)
            : base(ServerPacketHeader.VideoOffersRewardsMessageComposer)
        {
            base.WriteString(Type);
            base.WriteInteger(Id);
            base.WriteString(Message);
            base.WriteString("");
        }
    }
}

 