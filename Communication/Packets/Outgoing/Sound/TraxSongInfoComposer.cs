namespace Plus.Communication.Packets.Outgoing.Sound
{
    class TraxSongInfoComposer : ServerPacket
    {
        public TraxSongInfoComposer()
            : base(ServerPacketHeader.TraxSongInfoMessageComposer)
        {
            WriteInteger(0);//Count
        }
    }
}
