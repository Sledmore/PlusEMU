using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.GameCenter
{
    class PlayableGamesComposer : ServerPacket
    {
        public PlayableGamesComposer(int GameID)
            : base(ServerPacketHeader.PlayableGamesMessageComposer)
        {
            base.WriteInteger(GameID);
            base.WriteInteger(0);
        }
    }
}
