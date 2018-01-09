namespace Plus.Communication.Packets.Outgoing.Inventory.Achievements
{
    class AchievementScoreComposer : ServerPacket
    {
        public AchievementScoreComposer(int achScore)
            : base(ServerPacketHeader.AchievementScoreMessageComposer)
        {
            WriteInteger(achScore);
        }
    }
}
