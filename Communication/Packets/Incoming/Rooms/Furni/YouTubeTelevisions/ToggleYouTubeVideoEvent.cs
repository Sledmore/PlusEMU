using Plus.Communication.Packets.Outgoing.Rooms.Furni.YouTubeTelevisions;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni.YouTubeTelevisions
{
    class ToggleYouTubeVideoEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();//Item Id
            string VideoId = Packet.PopString(); //Video ID

            Session.SendPacket(new GetYouTubeVideoComposer(ItemId, VideoId));
        }
    }
}