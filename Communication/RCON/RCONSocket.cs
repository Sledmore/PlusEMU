using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using Plus.Communication.Rcon.Commands;

namespace Plus.Communication.Rcon
{
    public class RconSocket
    {
        private readonly Socket _musSocket;
        private readonly List<string> _allowedConnections;
        private readonly CommandManager _commands;

        public RconSocket(string host, int port, IEnumerable<string> allowedConnections)
        {
            _allowedConnections = new List<string>();
            foreach (string ipAddress in allowedConnections)
            {
                _allowedConnections.Add(ipAddress);
            }

            try
            {
                _musSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _musSocket.Bind(new IPEndPoint(IPAddress.Parse(host), port)); // SHould be host?
                _musSocket.Listen(0);
                _musSocket.BeginAccept(OnCallBack, _musSocket);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Could not set up Rcon socket:\n" + e);
            }

            _commands = new CommandManager();
        }

        private void OnCallBack(IAsyncResult iAr)
        {
            try
            {
                Socket socket = ((Socket)iAr.AsyncState).EndAccept(iAr);

                string ip = socket.RemoteEndPoint.ToString().Split(':')[0];
                if (_allowedConnections.Contains(ip))
                {
                    new RconConnection(socket);
                }
                else
                {
                    socket.Close();
                }
            }
            catch (Exception)
            {
                // ignored
            }

            _musSocket.BeginAccept(OnCallBack, _musSocket);
        }

        public CommandManager GetCommands()
        {
            return _commands;
        }
    }
}