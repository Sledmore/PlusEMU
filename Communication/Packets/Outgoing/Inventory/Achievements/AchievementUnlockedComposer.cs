using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Achievements;

namespace Plus.Communication.Packets.Outgoing.Inventory.Achievements
{
    class AchievementUnlockedComposer : ServerPacket
    {
        public AchievementUnlockedComposer(Achievement Achievement, int Level, int PointReward, int PixelReward)
            : base(ServerPacketHeader.AchievementUnlockedMessageComposer)
        {
            base.WriteInteger(Achievement.Id); // Achievement ID
            base.WriteInteger(Level); // Achieved level
            base.WriteInteger(144); // Unknown. Random useless number.
           base.WriteString(Achievement.GroupName + Level); // Achieved name
            base.WriteInteger(PointReward); // Point reward
            base.WriteInteger(PixelReward); // Pixel reward
            base.WriteInteger(0); // Unknown.
            base.WriteInteger(10); // Unknown.
            base.WriteInteger(21); // Unknown. (Extra reward?)
           base.WriteString(Level > 1 ? Achievement.GroupName + (Level - 1) : string.Empty);
           base.WriteString(Achievement.Category);
            base.WriteBoolean(true);
        }
    }
}
