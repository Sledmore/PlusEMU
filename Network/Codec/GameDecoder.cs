using System;
using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.GameClients;

namespace Plus.Network.Codec
{
    public class GameDecoder : MessageToMessageDecoder<IByteBuffer>
    {
        protected override void Decode(IChannelHandlerContext context, IByteBuffer message, List<object> output)
        {

            if (message.ReadableBytes < 5) return;

            int length = message.ReadInt();

            if (message.ReadableBytes < length)
            {
                message.ResetReaderIndex();
                return;
            }

            ClientPacket pkt = new ClientPacket(message);

            if (PlusEnvironment.GetGame().GetClientManager().TryGetClient(context.Channel.Id, out GameClient session))
            {
                PlusEnvironment.GetGame().GetPacketManager().TryExecutePacket(session, pkt);
            }

            output.Add(message);
        }
    }
}