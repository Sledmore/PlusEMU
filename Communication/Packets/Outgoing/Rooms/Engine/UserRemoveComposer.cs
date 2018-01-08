namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class UserRemoveComposer : ServerPacket
    {
        public UserRemoveComposer(int Id)
            : base(ServerPacketHeader.UserRemoveMessageComposer)
        {
           base.WriteString(Id.ToString());
        }
    }
}
