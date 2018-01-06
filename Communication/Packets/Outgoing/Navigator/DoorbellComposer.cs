namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class DoorbellComposer : ServerPacket
    {
        public DoorbellComposer(string username)
            : base(ServerPacketHeader.DoorbellMessageComposer)
        {
           WriteString(username);
        }
    }
}
