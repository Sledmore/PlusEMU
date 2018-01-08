namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class RoomInviteComposer : ServerPacket
    {
        public RoomInviteComposer(int SenderId, string Text)
            : base(ServerPacketHeader.RoomInviteMessageComposer)
        {
            base.WriteInteger(SenderId);
           base.WriteString(Text);
        }
    }
}
