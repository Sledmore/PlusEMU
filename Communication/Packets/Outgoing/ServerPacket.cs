using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DotNetty.Buffers;
using Plus.Communication.Interfaces;

namespace Plus.Communication.Packets.Outgoing
{
    public class ServerPacket
    {
        public IByteBuffer Buffer { get; }
        protected short Id { get; }
        private bool Finalized { get; set; }

        public ServerPacket(short id)
        {
            Buffer = Unpooled.Buffer(6);
            Id = id;
            Buffer.WriteInt(0);
            Buffer.WriteShort(id);
        }

        public void WriteByte(byte b) =>
            Buffer.WriteByte(b);

        public void WriteByte(int b) =>
            Buffer.WriteByte((byte) b);

        public void WriteDouble(double d) =>
            WriteString(d.ToString());

        public void WriteString(string s) // d
        {
            Buffer.WriteShort(s.Length);
            Buffer.WriteBytes(Encoding.Default.GetBytes(s));
        }

        public void WriteShort(int s) =>
            Buffer.WriteShort(s);

        public void WriteInteger(int i) =>
            Buffer.WriteInt(i);

        public void WriteBoolean(bool b) =>
            Buffer.WriteByte(b ? 1 : 0);

        public int Length => Buffer.WriterIndex - 4;

        public IByteBuffer FinalizedBuffer
        {
            get
            {
                if (Finalized) return Buffer;
                
                Buffer.SetInt(0, Length);
                Finalized = true;

                return Buffer;
            }
        }
    }
}