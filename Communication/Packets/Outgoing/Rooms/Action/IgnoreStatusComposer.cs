namespace Plus.Communication.Packets.Outgoing.Rooms.Action
{
    class IgnoreStatusComposer : ServerPacket
    {
        public IgnoreStatusComposer(int Status, string Username)
            : base(ServerPacketHeader.IgnoreStatusMessageComposer)
        {
            base.WriteInteger(Status);
           base.WriteString(Username);
        }
    }
}
