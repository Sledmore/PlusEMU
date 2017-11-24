using System;
using System.Text;

using Plus.Utilities;
using Plus.Communication.Packets.Incoming;

namespace Plus.Communication.Packets.Incoming
{
    public class ClientPacket
    {
        private byte[] Body;
        private int MessageId;
        private int Pointer;

        public ClientPacket(int messageID, byte[] body)
        {
            Init(messageID, body);
        }

        public int Id
        {
            get { return MessageId; }
        }

        public int RemainingLength
        {
            get { return Body.Length - Pointer; }
        }

        public int Header
        {
            get { return MessageId; }
        }

        public void Init(int messageID, byte[] body)
        {
            if (body == null)
                body = new byte[0];

            MessageId = messageID;
            Body = body;

            Pointer = 0;
        }

        public override string ToString()
        {
            return "[" + Header + "] BODY: " + (PlusEnvironment.GetDefaultEncoding().GetString(Body).Replace(Convert.ToChar(0).ToString(), "[0]"));
        }

        public void AdvancePointer(int i)
        {
            Pointer += i*4;
        }

        public byte[] ReadBytes(int Bytes)
        {
            if (Bytes > RemainingLength)
                Bytes = RemainingLength;

            var data = new byte[Bytes];

            for (int i = 0; i < Bytes; i++)
                data[i] = Body[Pointer++];

            return data;
        }

        public byte[] PlainReadBytes(int Bytes)
        {
            if (Bytes > RemainingLength)
                Bytes = RemainingLength;

            var data = new byte[Bytes];

            for (int x = 0, y = Pointer; x < Bytes; x++, y++)
            {
                data[x] = Body[y];
            }

            return data;
        }

        public byte[] ReadFixedValue()
        {
            int len = HabboEncoding.DecodeInt16(ReadBytes(2));
            return ReadBytes(len);
        }

        public string PopString()
        {
            return PlusEnvironment.GetDefaultEncoding().GetString(ReadFixedValue());
        }

        public bool PopBoolean()
        {
            if (RemainingLength > 0 && Body[Pointer++] == Convert.ToChar(1))
            {
                return true;
            }

            return false;
        }

        public int PopInt()
        {
            if (RemainingLength < 1)
            {
                return 0;
            }

            byte[] Data = PlainReadBytes(4);

            Int32 i = HabboEncoding.DecodeInt32(Data);

            Pointer += 4;

            return i;
        }
    }
}