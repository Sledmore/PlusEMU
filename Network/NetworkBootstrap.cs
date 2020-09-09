using DotNetty.Buffers;
using DotNetty.Codecs;
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
            // TODO: make this configurable
            IEventLoopGroup bossGroup = new MultithreadEventLoopGroup(1);
            IEventLoopGroup workerGroup = new MultithreadEventLoopGroup(10);
            IEventLoopGroup channelGroup = new MultithreadEventLoopGroup(7);
            
            ServerBootstrap server = new ServerBootstrap()
                .Group(bossGroup, workerGroup)
                .Channel<TcpServerSocketChannel>()
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    channel.Pipeline.AddLast("xmlDecoder", new GamePolicyDecoder());
                    channel.Pipeline.AddLast("frameDecoder", new LengthFieldBasedFrameDecoder(500000, 0, 4, 0, 4));
                    channel.Pipeline.AddLast("gameEncoder", new GameEncoder());
                    channel.Pipeline.AddLast("gameDecoder", new GameDecoder());
                    channel.Pipeline.AddLast(channelGroup, "clientHandler", new NetworkChannelHandler());
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