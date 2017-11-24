namespace Plus.Communication.Packets.Outgoing.Rooms.Chat
{
    public class ShoutComposer : ServerPacket
    {
        public ShoutComposer(int VirtualId, string Message, int Emotion, int Colour)
            : base(ServerPacketHeader.ShoutMessageComposer)
        {
            base.WriteInteger(VirtualId);
           base.WriteString(Message);
            base.WriteInteger(Emotion);
            base.WriteInteger(Colour);
            base.WriteInteger(0);
            base.WriteInteger(-1);
        }
    }
}