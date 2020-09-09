namespace Plus.Communication.Packets.Outgoing.Rooms.Chat
{
    public class UserTypingComposer : MessageComposer
    {
        public int VirtualId { get; }
        public bool Typing { get; }

        public UserTypingComposer(int VirtualId, bool Typing)
            : base(ServerPacketHeader.UserTypingMessageComposer)
        {
            this.VirtualId = VirtualId;
            this.Typing = Typing;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(VirtualId);
            packet.WriteInteger(Typing ? 1 : 0);
        }
    }
}