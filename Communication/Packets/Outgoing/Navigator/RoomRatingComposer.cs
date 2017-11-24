using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class RoomRatingComposer : ServerPacket
    {
        public RoomRatingComposer(int Score, bool CanVote)
            : base(ServerPacketHeader.RoomRatingMessageComposer)
        {
            base.WriteInteger(Score);
            base.WriteBoolean(CanVote);
        }
    }
}
