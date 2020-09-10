namespace Plus.Communication.Packets.Outgoing.GameCenter
{
    class JoinQueueComposer : MessageComposer
    {
        public int GameId { get; }

        public JoinQueueComposer(int GameId)
            : base(ServerPacketHeader.JoinQueueMessageComposer)
        {
            this.GameId = GameId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(GameId);
        }
    }
}
