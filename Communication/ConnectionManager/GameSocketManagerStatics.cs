namespace Plus.Communication.ConnectionManager
{
    public static class GameSocketManagerStatics
    {
        public static readonly int BufferSize = 8192;
        public static readonly int MaxPacketSize = BufferSize - 4;
    }
}