namespace Plus.Communication.Packets.Outgoing.Rooms.Session
{
    class FlatAccessibleComposer : ServerPacket
    {
        public FlatAccessibleComposer(string Username)
            : base(ServerPacketHeader.FlatAccessibleMessageComposer)
        {
           WriteString(Username);
        }
    }
}
