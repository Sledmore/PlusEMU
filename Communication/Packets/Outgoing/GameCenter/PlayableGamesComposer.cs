namespace Plus.Communication.Packets.Outgoing.GameCenter
{
    class PlayableGamesComposer : MessageComposer
    {
        public int GameId { get; }

        public PlayableGamesComposer(int GameID)
            : base(ServerPacketHeader.PlayableGamesMessageComposer)
        {
            this.GameId = GameID;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(GameId);
            packet.WriteInteger(0);
        }
    }
}
