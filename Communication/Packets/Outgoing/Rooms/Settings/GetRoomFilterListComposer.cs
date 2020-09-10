using Plus.HabboHotel.Rooms;
using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.Rooms.Settings
{
    class GetRoomFilterListComposer : MessageComposer
    {
        public List<string> WordFilterList { get; }
        public GetRoomFilterListComposer(List<string> WordFilterList)
            : base(ServerPacketHeader.GetRoomFilterListMessageComposer)
        {
            this.WordFilterList = WordFilterList;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(WordFilterList.Count);
            foreach (string Word in WordFilterList)
            {
                packet.WriteString(Word);
            }
        }
    }
}
