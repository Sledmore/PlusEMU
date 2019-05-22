using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Plus.Communication.Packets.Incoming;

namespace Plus.Network.Codec
{
    public class GameDecoder : MessageToMessageDecoder<IByteBuffer>
    {
        protected override void Decode(IChannelHandlerContext context, IByteBuffer message, List<object> output)
        {
            message.MarkReaderIndex();

            if (message.ReadableBytes < 6) return;

            var delimeter = message.ReadByte();
            message.ResetReaderIndex();

            if (delimeter == 60)
            {
                var policy = "<?xml version=\"1.0\"?>\r\n"
                             + "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n"
                             + "<cross-domain-policy>\r\n"
                             + "<allow-access-from domain=\"*\" to-ports=\"*\" />\r\n"
                             + "</cross-domain-policy>\0)";
                context.WriteAndFlushAsync(Unpooled.CopiedBuffer(Encoding.Default.GetBytes(policy)));
            }
            else
            {
                message.MarkReaderIndex();
                var length = message.ReadInt();
                if (message.ReadableBytes < length)
                {
                    message.ResetReaderIndex();
                    return;
                }

                var newBuf = message.ReadBytes(length);

                if (length < 0) return;
                var packet = new ClientPacket(newBuf);
                if (PlusEnvironment.GetGame().GetClientManager().TryGetClient(context.Channel.Id, out var client))
                    PlusEnvironment.GetGame().GetPacketManager().TryExecutePacket(client, packet);
                output.Add(message);
            }
        }
    }
}