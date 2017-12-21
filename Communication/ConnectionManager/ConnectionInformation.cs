using System;
using System.Net.Sockets;
using System.Text;

namespace Plus.Communication.ConnectionManager
{
    public class ConnectionInformation : IDisposable
    {
        #region declares

        /// <summary>
        ///     Is used when a connection state changes
        /// </summary>
        /// <param name="state">The new state of the connection</param>
        public delegate void ConnectionChange(ConnectionInformation information, ConnectionState state);

        private static bool disableSend = false;
        private static bool disableReceive = false;

        /// <summary>
        ///     Buffer of the connection
        /// </summary>
        private readonly byte[] buffer;

        /// <summary>
        ///     The id of this connection
        /// </summary>
        private readonly int connectionID;

        /// <summary>
        ///     The socket this connection is based upon
        /// </summary>
        private readonly Socket dataSocket;

        /// <summary>
        ///     The ip of this connection
        /// </summary>
        private readonly string ip;

        private readonly AsyncCallback sendCallback;

        /// <summary>
        ///     Boolean indicating of this instance is connected to the user or not
        /// </summary>
        private bool isConnected;

        /// <summary>
        ///     This item contains the data parser for the connection
        /// </summary>
        public IDataParser parser { get; set; }

        /// <summary>
        ///     Is triggered when the user connects/disconnects
        /// </summary>
        public event ConnectionChange connectionChanged;

        #endregion

        #region constructor

        /// <summary>
        ///     Creates a new Connection witht he given information
        /// </summary>
        /// <param name="dataStream">The Socket of the connection</param>
        /// <param name="connectionID">The id of the connection</param>
        public ConnectionInformation(int connectionID, Socket dataStream, SocketManager manager, IDataParser parser, string ip)
        {
            this.parser = parser;
            buffer = new byte[GameSocketManagerStatics.BUFFER_SIZE];
            //this.manager = manager;
            dataSocket = dataStream;
            dataSocket.SendBufferSize = GameSocketManagerStatics.BUFFER_SIZE;
            this.ip = ip;
            sendCallback = sentData;
            this.connectionID = connectionID;
            if (connectionChanged != null)
                connectionChanged.Invoke(this, ConnectionState.Open);

        }

        /// <summary>
        ///     Starts this item packet processor
        ///     MUST be called before sending data
        /// </summary>
        public void startPacketProcessing()
        {
            if (!isConnected)
            {
                isConnected = true;
                //Out.writeLine("Starting packet processsing of client [" + this.connectionID + "]", Out.logFlags.lowLogLevel);
                try
                {
                    dataSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, incomingDataPacket, dataSocket);
                }
                catch
                {
                    disconnect();
                }
            }
        }

        #endregion

        #region getters

        /// <summary>
        ///     Returns the ip of the current connection
        /// </summary>
        /// <returns>The ip of this connection</returns>
        public string GetIP()
        {
            return ip;
        }

        /// <summary>
        ///     Returns the connection id
        /// </summary>
        /// <returns>The id of the connection</returns>
        public int GetConnectionID()
        {
            return connectionID;
        }

        #endregion

        #region methods

        #region connection management

        /// <summary>
        ///     Disposes the current item
        /// </summary>
        public void Dispose()
        {
            if (isConnected)
            {
                disconnect();
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Disconnects the current connection
        /// </summary>
        public void disconnect()
        {
            try
            {
                if (isConnected)
                {
                    isConnected = false;

                    //Out.writeLine("Connection [" + this.connectionID + "] has been disconnected", Out.logFlags.BelowStandardlogLevel);
                    try
                    {
                        if (dataSocket != null && dataSocket.Connected)
                        {
                            dataSocket.Shutdown(SocketShutdown.Both);
                            dataSocket.Close();
                        }
                    }
                    catch
                    {
                    }
                    dataSocket.Dispose();
                    parser.Dispose();

                    try
                    {
                        if (connectionChanged != null)
                            connectionChanged.Invoke(this, ConnectionState.Closed);
                    }
                    catch
                    {
                    }
                    connectionChanged = null;
                }
                else
                {
                    //Out.writeLine("Connection [" + this.connectionID + "] has already been disconnected - ignoring disconnect call", Out.logFlags.BelowStandardlogLevel);
                }
            }
            catch
            {
            }
        }

        #endregion

        #region data receiving

        /// <summary>
        ///     Receives a packet of data and processes it
        /// </summary>
        /// <param name="iAr">The interface of an async result</param>
        private void incomingDataPacket(IAsyncResult iAr)
        {
            //Out.writeLine("Packet received from client [" + this.connectionID + "]", Out.logFlags.lowLogLevel);
            int bytesReceived;
            try
            {
                //The amount of bytes received in the packet
                bytesReceived = dataSocket.EndReceive(iAr);
            }
            catch //(Exception e)
            {
                disconnect();
                return;
            }

            if (bytesReceived == 0)
            {
                disconnect();
                return;
            }

            try
            {
                if (!disableReceive)
                {
                    var packet = new byte[bytesReceived];
                    Array.Copy(buffer, packet, bytesReceived);
                    handlePacketData(packet);
                }
            }
            catch //(Exception e)
            {
                disconnect();
            }
            finally
            {
                try
                {
                    //and we keep looking for the next packet

                    dataSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, incomingDataPacket, dataSocket);
                }
                catch //(Exception e)
                {
                    disconnect();
                }
            }
        }

        /// <summary>
        ///     Handles packet data
        /// </summary>
        /// <param name="packet">The data received by the </param>
        private void handlePacketData(byte[] packet)
        {
            if (parser != null)
            {
                parser.HandlePacketData(packet);
            }
        }

        #endregion

        #region data sending

        public void SendData(byte[] packet)
        {
            try
            {
                if (!isConnected || disableSend)
                    return;

                //if(RC4Server == null)
                //  RC4Server = new ARC4(new byte[] { 10 });
                //if (RC4Server != null)
                //packet = RC4Server.Parse(packet);

                string packetData = Encoding.Default.GetString(packet);
                //Console.WriteLine(string.Format("Data from server => [{0}]", packetData));
                dataSocket.BeginSend(packet, 0, packet.Length, 0, sendCallback, null);
            }
            catch
            {
                disconnect();
            }
        }

        /// <summary>
        ///     Same as sendData
        /// </summary>
        /// <param name="iAr">The a-synchronious interface</param>
        private void sentData(IAsyncResult iAr)
        {
            try
            {
                 dataSocket.EndSend(iAr);
            }
            catch
            {
                disconnect();
            }
        }
        #endregion

        #endregion
    }
}