using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.GameCenter
{
    class JoinQueueComposer : ServerPacket
    {
        public JoinQueueComposer(int GameId)
            : base(ServerPacketHeader.JoinQueueMessageComposer)
        {
            base.WriteInteger(GameId);
        }
    }
}
