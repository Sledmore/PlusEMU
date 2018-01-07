namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.YouTubeTelevisions
{
    class GetYouTubeVideoComposer : ServerPacket
    {
        public GetYouTubeVideoComposer(int ItemId, string YouTubeVideo)
            : base(ServerPacketHeader.GetYouTubeVideoMessageComposer)
        {
            base.WriteInteger(ItemId);
           base.WriteString(YouTubeVideo);//"9Ht5RZpzPqw");
            base.WriteInteger(0);//Start seconds
            base.WriteInteger(0);//End seconds
            base.WriteInteger(0);//State
        }
    }
}
