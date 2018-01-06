using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class PopularRoomTagsResultComposer : ServerPacket
    {
        public PopularRoomTagsResultComposer(ICollection<KeyValuePair<string, int>> tags)
            : base(ServerPacketHeader.PopularRoomTagsResultMessageComposer)
        {
            WriteInteger(tags.Count);
            foreach (KeyValuePair<string, int> tag in tags)
            {
                WriteString(tag.Key);
                WriteInteger(tag.Value);
            }
        }
    }
}
