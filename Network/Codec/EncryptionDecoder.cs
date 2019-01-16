using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using HabboEncryption;
using Plus.Communication.Encryption.Crypto.Prng;

namespace Plus.Network.Codec
{
    public class EncryptionDecoder : ByteToMessageDecoder
    {
        private ARC4 Arc4 { get; }

        public EncryptionDecoder(byte[] key)
        {
            Arc4 = new ARC4(key);
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            output.Add(Arc4.Decipher(input));
        }
    }
}