using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Plus.Communication.Packets.Outgoing;

namespace Plus.Network.Codec
{
    public class GameEncoder : MessageToByteEncoder<MessageComposer>
    {
        protected override void Encode(IChannelHandlerContext context, MessageComposer message, IByteBuffer output)
        {
            ServerPacket packet = message.WriteMessage(output);
            packet.FinalizeBuffer();
        }
    }
}