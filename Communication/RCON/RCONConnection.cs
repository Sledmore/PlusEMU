using System;
using System.Text;
using System.Net.Sockets;
using log4net;

namespace Plus.Communication.Rcon
{
    public class RconConnection
    {
        private Socket _socket;
        private byte[] _buffer = new byte[1024];

        private static readonly ILog log = LogManager.GetLogger("Plus.Communication.Rcon.RconConnection");

        public RconConnection(Socket socket)
        {
            this._socket = socket;

            try
            {
                this._socket.BeginReceive(this._buffer, 0, this._buffer.Length, SocketFlags.None, OnCallBack, this._socket);
            }
            catch { Dispose(); }
        }

        public void OnCallBack(IAsyncResult iAr)
        {
            try
            {
                int bytes = 0;
                if (!int.TryParse(_socket.EndReceive(iAr).ToString(), out bytes))
                {
                    Dispose();
                    return;
                }

                string data = Encoding.Default.GetString(_buffer, 0, bytes);
                if (!PlusEnvironment.GetRconSocket().GetCommands().Parse(data))
                {
                    log.Error("Failed to execute a MUS command. Raw data: " + data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Dispose();
        }

        public void Dispose()
        {
            if (this._socket != null)
            {
                this._socket.Shutdown(SocketShutdown.Both);
                this._socket.Close();
                this._socket.Dispose();
            }
            
            this._socket = null;
            this._buffer = null;
        }
    }
}
