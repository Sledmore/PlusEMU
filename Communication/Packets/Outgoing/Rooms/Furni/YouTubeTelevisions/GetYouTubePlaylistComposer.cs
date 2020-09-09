using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Items.Televisions;

namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.YouTubeTelevisions
{
    class GetYouTubePlaylistComposer : MessageComposer
    {
        public int ItemId { get; }
        public ICollection<TelevisionItem> Videos { get; }

        public GetYouTubePlaylistComposer(int ItemId, ICollection<TelevisionItem> Videos)
            : base(ServerPacketHeader.GetYouTubePlaylistMessageComposer)
        {
            this.ItemId = ItemId;
            this.Videos = Videos;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(ItemId);
            packet.WriteInteger(Videos.Count);
            foreach (TelevisionItem Video in Videos.ToList())
            {
                packet.WriteString(Video.YouTubeId);
                packet.WriteString(Video.Title);//Title
                packet.WriteString(Video.Description);//Description
            }
            packet.WriteString("");
        }
    }
}
