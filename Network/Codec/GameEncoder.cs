using System.Collections.Generic;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Plus.Communication.Packets.Outgoing;

namespace Plus.Network.Codec
{
    public class GameEncoder : MessageToMessageEncoder<ServerPacket>
    {
        protected override void Encode(IChannelHandlerContext context, ServerPacket message, List<object> output)
        {
            output.Add(message.FinalizedBuffer);
        }
    }
}