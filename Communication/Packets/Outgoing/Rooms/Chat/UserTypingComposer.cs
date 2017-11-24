namespace Plus.Communication.Packets.Outgoing.Rooms.Chat
{
    public class UserTypingComposer : ServerPacket
    {
        public UserTypingComposer(int VirtualId, bool Typing)
            : base(ServerPacketHeader.UserTypingMessageComposer)
        {
            base.WriteInteger(VirtualId);
            base.WriteInteger(Typing ? 1 : 0);
        }
    }
}