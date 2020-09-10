using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Network.Codec
{
    class GamePolicyDecoder : ByteToMessageDecoder
    {
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            input.MarkReaderIndex();
            if (input.ReadableBytes < 1) return;
            byte delimiter = input.ReadByte();
            input.ResetReaderIndex();
            if (delimiter == 0x3C)
            {
                var policy = "<?xml version=\"1.0\"?>\r\n"
                             + "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n"
                             + "<cross-domain-policy>\r\n"
                             + "<allow-access-from domain=\"*\" to-ports=\"*\" />\r\n"
                             + "</cross-domain-policy>\0)";
                context.WriteAndFlushAsync(Unpooled.CopiedBuffer(Encoding.Default.GetBytes(policy)));
            } else
            {
                context.Channel.Pipeline.Remove(this);
            }
        }
    }
}
