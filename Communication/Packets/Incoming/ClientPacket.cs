using System;
using System.Text;
using DotNetty.Buffers;
using Plus.Utilities;

namespace Plus.Communication.Packets.Incoming
{
    public class ClientPacket
    {
        private IByteBuffer buffer;
        public short Id { get; }

        public ClientPacket(IByteBuffer buf)
        {
            buffer = buf;
            Id = buffer.ReadShort();
        }

        public string PopString()
        {
            int length = buffer.ReadShort();
            IByteBuffer data = buffer.ReadBytes(length);
            return Encoding.Default.GetString(data.Array);
        }

        public int PopInt() =>
            buffer.ReadInt();

        public bool PopBoolean() =>
            buffer.ReadByte() == 1;
    }
}