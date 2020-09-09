namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.YouTubeTelevisions
{
    class GetYouTubeVideoComposer : MessageComposer
    {
        public int ItemId { get; }
        public string YouTubeVideo { get; }

        public GetYouTubeVideoComposer(int ItemId, string YouTubeVideo)
            : base(ServerPacketHeader.GetYouTubeVideoMessageComposer)
        {
            this.ItemId = ItemId;
            this.YouTubeVideo = YouTubeVideo;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(ItemId);
            packet.WriteString(YouTubeVideo);//"9Ht5RZpzPqw");
            packet.WriteInteger(0);//Start seconds
            packet.WriteInteger(0);//End seconds
            packet.WriteInteger(0);//State
        }
    }
}
