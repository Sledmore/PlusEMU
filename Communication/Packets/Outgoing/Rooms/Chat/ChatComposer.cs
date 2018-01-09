namespace Plus.Communication.Packets.Outgoing.Rooms.Chat
{
    public class ChatComposer : ServerPacket
    {
        public ChatComposer(int VirtualId, string Message, int Emotion, int Colour)
            : base(ServerPacketHeader.ChatMessageComposer)
        {
            WriteInteger(VirtualId);
           WriteString(Message);
            WriteInteger(Emotion);
            WriteInteger(Colour);
            WriteInteger(0);
            WriteInteger(-1);
        }
    }
}