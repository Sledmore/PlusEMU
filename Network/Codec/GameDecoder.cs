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
            try
            {
                IByteBuffer newBuf = message.ReadBytes(message.ReadableBytes);
                short id = newBuf.ReadShort();
                var packet = new ClientPacket(id, newBuf);
               
                output.Add(packet);
            }catch (Exception e)
            {
                
            }
                
        }
    }
}