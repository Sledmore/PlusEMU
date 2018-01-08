namespace Plus.Communication.Packets.Outgoing.Rooms.Chat
{
    public class WhisperComposer : ServerPacket
    {
        public WhisperComposer(int VirtualId, string Text, int Emotion, int Colour)
            : base(ServerPacketHeader.WhisperMessageComposer)
        {
            WriteInteger(VirtualId);
           WriteString(Text);
            WriteInteger(Emotion);
            WriteInteger(Colour);

            WriteInteger(0);
            WriteInteger(-1);
        }
    }
}