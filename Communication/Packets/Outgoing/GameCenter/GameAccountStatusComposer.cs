namespace Plus.Communication.Packets.Outgoing.GameCenter
{
    class GameAccountStatusComposer : ServerPacket
    {
        public GameAccountStatusComposer(int GameID)
            : base(ServerPacketHeader.GameAccountStatusMessageComposer)
        {
            WriteInteger(GameID);
            WriteInteger(-1); // Games Left
            WriteInteger(0);//Was 16?
        }
    }
}