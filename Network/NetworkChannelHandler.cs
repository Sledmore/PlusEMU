using System;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;

namespace Plus.Network
{
    public class NetworkChannelHandler : ChannelHandlerAdapter
    {
        public override void ChannelRegistered(IChannelHandlerContext context)
        {
            PlusEnvironment.GetGame().GetClientManager().CreateAndStartClient(context);
        }

        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            PlusEnvironment.GetGame().GetClientManager().DisposeConnection(context.Channel.Id);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        { }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
        }
    }
}