using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using Plus.Communication.RCON.Commands;

namespace Plus.Communication.RCON
{
    public class RCONSocket
    {
        private Socket _musSocket;

        private string _musIP;
        private int _musPort;

        private List<string> _allowedConnections;
        private CommandManager _commands;

        public RCONSocket(string musIP, int musPort, string[] allowedConnections)
        {
            this._musIP = musIP;
            this._musPort = musPort;

            this._allowedConnections = new List<string>();
            foreach (string ipAddress in allowedConnections)
            {
                this._allowedConnections.Add(ipAddress);
            }

            try
            {
                this._musSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this._musSocket.Bind(new IPEndPoint(IPAddress.Any, this._musPort));
                this._musSocket.Listen(0);
                this._musSocket.BeginAccept(OnCallBack, this._musSocket);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Could not set up RCON socket:\n" + e);
            }

            this._commands = new CommandManager();
        }

        private void OnCallBack(IAsyncResult iAr)
        {
            try
            {
                Socket socket = ((Socket)iAr.AsyncState).EndAccept(iAr);

                string ip = socket.RemoteEndPoint.ToString().Split(':')[0];
                if (this._allowedConnections.Contains(ip))
                {
                    new RCONConnection(socket);
                }
                else
                {
                    socket.Close();
                }
            }
            catch (Exception)
            {
            }

            this._musSocket.BeginAccept(OnCallBack, _musSocket);
        }

        public CommandManager GetCommands()
        {
            return this._commands;
        }
    }
}