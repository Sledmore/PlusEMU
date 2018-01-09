using Plus.HabboHotel.Achievements;

namespace Plus.Communication.Packets.Outgoing.Inventory.Achievements
{
    class AchievementUnlockedComposer : ServerPacket
    {
        public AchievementUnlockedComposer(Achievement Achievement, int Level, int PointReward, int PixelReward)
            : base(ServerPacketHeader.AchievementUnlockedMessageComposer)
        {
            WriteInteger(Achievement.Id); // Achievement ID
            WriteInteger(Level); // Achieved level
            WriteInteger(144); // Unknown. Random useless number.
           WriteString(Achievement.GroupName + Level); // Achieved name
            WriteInteger(PointReward); // Point reward
            WriteInteger(PixelReward); // Pixel reward
            WriteInteger(0); // Unknown.
            WriteInteger(10); // Unknown.
            WriteInteger(21); // Unknown. (Extra reward?)
           WriteString(Level > 1 ? Achievement.GroupName + (Level - 1) : string.Empty);
           WriteString(Achievement.Category);
            WriteBoolean(true);
        }
    }
}
