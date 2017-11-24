using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Inventory.Achievements
{
    class AchievementScoreComposer : ServerPacket
    {
        public AchievementScoreComposer(int achScore)
            : base(ServerPacketHeader.AchievementScoreMessageComposer)
        {
            base.WriteInteger(achScore);
        }
    }
}
