using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Items.Televisions;

namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.YouTubeTelevisions
{
    class GetYouTubePlaylistComposer : ServerPacket
    {
        public GetYouTubePlaylistComposer(int ItemId, ICollection<TelevisionItem> Videos)
            : base(ServerPacketHeader.GetYouTubePlaylistMessageComposer)
        {
            WriteInteger(ItemId);
            WriteInteger(Videos.Count);
            foreach (TelevisionItem Video in Videos.ToList())
            {
               WriteString(Video.YouTubeId);
               WriteString(Video.Title);//Title
               WriteString(Video.Description);//Description
            }
           WriteString("");
        }
    }
}
