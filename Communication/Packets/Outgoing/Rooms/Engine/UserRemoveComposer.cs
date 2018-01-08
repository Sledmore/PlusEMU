namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class UserRemoveComposer : ServerPacket
    {
        public UserRemoveComposer(int Id)
            : base(ServerPacketHeader.UserRemoveMessageComposer)
        {
           WriteString(Id.ToString());
        }
    }
}
