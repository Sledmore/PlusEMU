using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Concurrency;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using log4net;
using Plus.Network.Codec;
using System.Net;
using System.Threading.Tasks;

namespace Plus.Network
{
    public class NetworkBootstrap
    {
        private string Host { get; }
        private int Port { get; }
        private IEventLoopGroup BossGroup { get; set; }
        private IEventLoopGroup WorkerGroup { get; set; }
        private IEventExecutorGroup ChannelGroup { get; set; }
        private ServerBootstrap ServerBootstrap { get; set; }
        private IChannel ServerChannel { get; set; }
        private static readonly ILog log = LogManager.GetLogger(typeof(NetworkBootstrap));

        public NetworkBootstrap(string host, int port)
        {
            Host = host;
            Port = port;
        }

        public async Task InitAsync()
        {
            BossGroup = new MultithreadEventLoopGroup(int.Parse(PlusEnvironment.GetConfig().data["game.tcp.acceptGroupThreads"]));
            WorkerGroup = new MultithreadEventLoopGroup(int.Parse(PlusEnvironment.GetConfig().data["game.tcp.ioGroupThreads"]));
            ChannelGroup = new MultithreadEventLoopGroup(int.Parse(PlusEnvironment.GetConfig().data["game.tcp.channelGroupThreads"]));
            
            ServerBootstrap = new ServerBootstrap()
                .Group(BossGroup, WorkerGroup)
                .Channel<TcpServerSocketChannel>()
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    channel.Pipeline.AddLast("xmlDecoder", new GamePolicyDecoder());
                    channel.Pipeline.AddLast("frameDecoder", new LengthFieldBasedFrameDecoder(500000, 0, 4, 0, 4));
                    channel.Pipeline.AddLast("gameEncoder", new GameEncoder());
                    channel.Pipeline.AddLast("gameDecoder", new GameDecoder());
                    channel.Pipeline.AddLast(ChannelGroup, "clientHandler", new NetworkChannelHandler());
                }))
                .ChildOption(ChannelOption.TcpNodelay, true)
                .ChildOption(ChannelOption.SoKeepalive, true)
                .ChildOption(ChannelOption.SoRcvbuf, 1024)
                .ChildOption(ChannelOption.RcvbufAllocator, new FixedRecvByteBufAllocator(1024))
                .ChildOption(ChannelOption.Allocator, UnpooledByteBufferAllocator.Default);
            ServerChannel = await ServerBootstrap.BindAsync(IPAddress.Parse(Host), Port);
            log.Info($"Game Server listening on {((IPEndPoint)ServerChannel.LocalAddress).Address.MapToIPv4()}:{Port}");
        }

        /// <summary>
        ///     Close all channels and disconnects clients
        /// </summary>
        /// <returns></returns>
        public async Task Shutdown()
        {
            await ServerChannel.CloseAsync();
        }

        /// <summary>
        ///     Shutdown all workers of netty
        /// </summary>
        /// <returns></returns>
        public void ShutdownWorkers()
        {
            BossGroup.ShutdownGracefullyAsync();
            WorkerGroup.ShutdownGracefullyAsync();
            ChannelGroup.ShutdownGracefullyAsync();
        }
    }
}