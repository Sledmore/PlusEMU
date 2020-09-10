using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Mime;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Common;
using Plus.Communication.Interfaces;

namespace Plus.Communication.Packets.Outgoing
{
    public class ServerPacket : IByteBufferHolder
    {
        public IByteBuffer Buffer { get; }
        protected short Id { get; }

        public ServerPacket(short id, IByteBuffer body)
        {
            Buffer = body;
            Id = id;
            if(body.WriterIndex == 0)
            {
                Buffer.WriteInt(-1);
                Buffer.WriteShort(id);
            }
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

        public IByteBuffer Content => Buffer;

        public int ReferenceCount => Buffer.ReferenceCount;

        public void FinalizeBuffer()
        {
            Buffer.SetInt(0, Length);
        }

        public IByteBufferHolder Copy()
        {
            return new ServerPacket(Id, Buffer.Copy());
        }

        public IByteBufferHolder Duplicate()
        {
            return new ServerPacket(Id, Buffer.Duplicate());
        }

        public IByteBufferHolder RetainedDuplicate()
        {
            return new ServerPacket(Id, Buffer.RetainedDuplicate());
        }

        public IByteBufferHolder Replace(IByteBuffer content)
        {
            return new ServerPacket(Id, content);
        }

        public IReferenceCounted Retain()
        {
            return Buffer.Retain();
        }

        public IReferenceCounted Retain(int increment)
        {
            return Buffer.Retain(increment);
        }

        public IReferenceCounted Touch()
        {
            return Buffer.Touch();
        }

        public IReferenceCounted Touch(object hint)
        {
            return Buffer.Touch(hint);
        }

        public bool Release()
        {
            return Buffer.Release();
        }

        public bool Release(int decrement)
        {
            return Buffer.Release(decrement);
        }
    }
}