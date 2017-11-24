using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Rooms.Avatar
{
    class AvatarEffectComposer : ServerPacket
    {
        public AvatarEffectComposer(int playerID, int effectID)
            : base(ServerPacketHeader.AvatarEffectMessageComposer)
        {
            base.WriteInteger(playerID);
            base.WriteInteger(effectID);
            base.WriteInteger(0);
        }
    }
}