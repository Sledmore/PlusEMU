namespace Plus.Communication.Packets.Outgoing.Rooms.Chat
{
    public class UserTypingComposer : ServerPacket
    {
        public UserTypingComposer(int VirtualId, bool Typing)
            : base(ServerPacketHeader.UserTypingMessageComposer)
        {
            WriteInteger(VirtualId);
            WriteInteger(Typing ? 1 : 0);
        }
    }
}