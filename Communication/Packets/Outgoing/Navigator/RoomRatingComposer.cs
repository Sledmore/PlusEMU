namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class RoomRatingComposer : MessageComposer
    {
        public int Score { get; }
        public bool CanVote { get; }

        public RoomRatingComposer(int score, bool canVote)
            : base(ServerPacketHeader.RoomRatingMessageComposer)
        {
            this.Score = score;
            this.CanVote = canVote;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Score);
            packet.WriteBoolean(CanVote);
        }
    }
}
