using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class PopularRoomTagsResultComposer : ServerPacket
    {
        public PopularRoomTagsResultComposer(ICollection<KeyValuePair<string, int>> Tags)
            : base(ServerPacketHeader.PopularRoomTagsResultMessageComposer)
        {
            base.WriteInteger(Tags.Count);
            foreach (KeyValuePair<string, int> tag in Tags)
            {
               base.WriteString(tag.Key);
                base.WriteInteger(tag.Value);
            }
        }
    }
}
