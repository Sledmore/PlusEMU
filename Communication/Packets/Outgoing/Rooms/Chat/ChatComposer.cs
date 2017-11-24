using System;

namespace Plus.Communication.Packets.Outgoing.Rooms.Chat
{
    public class ChatComposer : ServerPacket
    {
        public ChatComposer(int VirtualId, string Message, int Emotion, int Colour)
            : base(ServerPacketHeader.ChatMessageComposer)
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