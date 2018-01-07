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
            base.WriteInteger(ItemId);
            base.WriteInteger(Videos.Count);
            foreach (TelevisionItem Video in Videos.ToList())
            {
               base.WriteString(Video.YouTubeId);
               base.WriteString(Video.Title);//Title
               base.WriteString(Video.Description);//Description
            }
           base.WriteString("");
        }
    }
}
