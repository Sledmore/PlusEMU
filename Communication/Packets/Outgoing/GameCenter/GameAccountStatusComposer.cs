namespace Plus.Communication.Packets.Outgoing.GameCenter
{
    class GameAccountStatusComposer : MessageComposer
    {
        public int GameId { get; }

        public GameAccountStatusComposer(int GameID)
            : base(ServerPacketHeader.GameAccountStatusMessageComposer)
        {
            this.GameId = GameID;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(GameId);
            packet.WriteInteger(-1); // Games Left
            packet.WriteInteger(0);//Was 16?
        }
    }
}