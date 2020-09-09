namespace Plus.Communication.Packets.Outgoing.Inventory.Achievements
{
    class AchievementScoreComposer : MessageComposer
    {
        public int AchievementScore { get; }

        public AchievementScoreComposer(int achScore)
            : base(ServerPacketHeader.AchievementScoreMessageComposer)
        {
            this.AchievementScore = achScore;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(AchievementScore);
        }
    }
}
