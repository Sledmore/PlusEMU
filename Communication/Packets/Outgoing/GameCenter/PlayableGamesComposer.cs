namespace Plus.Communication.Packets.Outgoing.GameCenter
{
    class PlayableGamesComposer : ServerPacket
    {
        public PlayableGamesComposer(int GameID)
            : base(ServerPacketHeader.PlayableGamesMessageComposer)
        {
            WriteInteger(GameID);
            WriteInteger(0);
        }
    }
}
