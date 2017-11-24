using System;

namespace Plus.Communication.ConnectionManager
{
    public interface IDataParser : IDisposable, ICloneable
    {
        void handlePacketData(byte[] packet);
    }
}