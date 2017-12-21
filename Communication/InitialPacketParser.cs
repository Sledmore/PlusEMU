using System;
using Plus.Communication.ConnectionManager;

namespace Plus.Communication
{
    public class InitialPacketParser : IDataParser
    {
        public delegate void NoParamDelegate();

        public byte[] currentData;

        public void HandlePacketData(byte[] packet)
        {
            if (packet[0] == 60 && PolicyRequest != null)
            {
                PolicyRequest.Invoke();
            }
            else if (packet[0] != 67 && SwitchParserRequest != null)
            {
                currentData = packet;
                SwitchParserRequest.Invoke();
            }
        }

        public void Dispose()
        {
            PolicyRequest = null;
            SwitchParserRequest = null;
            GC.SuppressFinalize(this);
        }

        public object Clone()
        {
            return new InitialPacketParser();
        }

        public event NoParamDelegate PolicyRequest;
        public event NoParamDelegate SwitchParserRequest;
    }
}