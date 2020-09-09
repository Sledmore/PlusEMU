namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class UserRemoveComposer : MessageComposer
    {
        public int UserId { get; }

        public UserRemoveComposer(int Id)
            : base(ServerPacketHeader.UserRemoveMessageComposer)
        {
            this.UserId = Id;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(UserId.ToString());
        }
    }
}
