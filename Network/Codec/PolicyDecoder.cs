using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Plus.Network.Codec
{
    public class PolicyDecoder : ByteToMessageDecoder
    {
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            char delimiter = (char) input.ReadByte();
            input.ResetReaderIndex();

            if (delimiter == '<')
            {
                string p = "<?xml version=\"1.0\"?>\r\n"
                           + "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n"
                           + "<cross-domain-policy>\r\n"
                           + "<allow-access-from domain=\"*\" to-ports=\"*\" />\r\n"
                           + "</cross-domain-policy>\0";
                context.WriteAndFlushAsync(Unpooled.CopiedBuffer(Encoding.Default.GetBytes(p))).Wait();
            }
            else
            {
                output.Add(input);
                context.Channel.Pipeline.Remove(this);
            }
        }
    }
}