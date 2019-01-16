using DotNetty.Buffers;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Plus.Network.Codec;

namespace Plus.Network
{
    public class NetworkBootstrap
    {
        private int Port { get; }

        public NetworkBootstrap(int port)
        {
            Port = port;
        }

        public void Init()
        {
            IEventLoopGroup bossGroup = new MultithreadEventLoopGroup(1);
            IEventLoopGroup workerGroup = new MultithreadEventLoopGroup(10);
            
            ServerBootstrap server = new ServerBootstrap()
                .Group(bossGroup, workerGroup)
                .Channel<TcpServerSocketChannel>()
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    channel.Pipeline.AddLast("gameEncoder", new GameEncoder());
                    channel.Pipeline.AddLast("policyDecoder", new PolicyDecoder());
                    channel.Pipeline.AddLast("gameDecoder", new GameDecoder());
                    channel.Pipeline.AddLast("clientHandler", new NetworkChannelHandler());
                }))
                .ChildOption(ChannelOption.TcpNodelay, true)
                .ChildOption(ChannelOption.SoKeepalive, true)
                .ChildOption(ChannelOption.SoRcvbuf, 1024)
                .ChildOption(ChannelOption.RcvbufAllocator, new FixedRecvByteBufAllocator(1024))
                .ChildOption(ChannelOption.Allocator, UnpooledByteBufferAllocator.Default);
            server.BindAsync(Port);
        }
    }
}