namespace Plus.Communication.Packets.Outgoing.Rooms.Chat
{
    public class ShoutComposer : ServerPacket
    {
        public ShoutComposer(int VirtualId, string Message, int Emotion, int Colour)
            : base(ServerPacketHeader.ShoutMessageComposer)
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