namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class FlatAccessDeniedComposer : ServerPacket
    {
        public FlatAccessDeniedComposer(string username)
            : base(ServerPacketHeader.FlatAccessDeniedMessageComposer)
        {
           WriteString(username);
        }
    }
}
