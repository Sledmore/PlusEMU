using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class PopularRoomTagsResultComposer : MessageComposer
    {
        public ICollection<KeyValuePair<string, int>> Tags { get; }

        public PopularRoomTagsResultComposer(ICollection<KeyValuePair<string, int>> tags)
            : base(ServerPacketHeader.PopularRoomTagsResultMessageComposer)
        {
            this.Tags = tags;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Tags.Count);
            foreach (KeyValuePair<string, int> tag in Tags)
            {
                packet.WriteString(tag.Key);
                packet.WriteInteger(tag.Value);
            }
        }
    }
}
