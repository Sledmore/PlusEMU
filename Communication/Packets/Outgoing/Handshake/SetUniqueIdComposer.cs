namespace Plus.Communication.Packets.Outgoing.Handshake
{
    class SetUniqueIdComposer : ServerPacket
    {
        public SetUniqueIdComposer(string Id)
            : base(ServerPacketHeader.SetUniqueIdMessageComposer)
        {
           WriteString(Id);
        }
    }
}
