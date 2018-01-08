namespace Plus.Communication.Packets.Outgoing.Misc
{
    class VideoOffersRewardsComposer : ServerPacket
    {
        public VideoOffersRewardsComposer(int Id, string Type, string Message)
            : base(ServerPacketHeader.VideoOffersRewardsMessageComposer)
        {
            WriteString(Type);
            WriteInteger(Id);
            WriteString(Message);
            WriteString("");
        }
    }
}

 