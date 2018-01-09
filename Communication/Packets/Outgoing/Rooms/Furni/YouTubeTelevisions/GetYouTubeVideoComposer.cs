namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.YouTubeTelevisions
{
    class GetYouTubeVideoComposer : ServerPacket
    {
        public GetYouTubeVideoComposer(int ItemId, string YouTubeVideo)
            : base(ServerPacketHeader.GetYouTubeVideoMessageComposer)
        {
            WriteInteger(ItemId);
           WriteString(YouTubeVideo);//"9Ht5RZpzPqw");
            WriteInteger(0);//Start seconds
            WriteInteger(0);//End seconds
            WriteInteger(0);//State
        }
    }
}
