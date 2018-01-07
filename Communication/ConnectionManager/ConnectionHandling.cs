using System;
using Plus.Communication.ConnectionManager;
using Plus.Communication;

namespace Plus.Communication.ConnectionManager
{
    public class ConnectionHandling
    {
        private readonly SocketManager _manager;

        public ConnectionHandling(int port, int maxConnections, int connectionsPerIp, bool enabeNagles)
        {
            _manager = new SocketManager();
            _manager.Init(port, maxConnections, connectionsPerIp, new InitialPacketParser(), !enabeNagles);
        }

        public void Init()
        {
            _manager.OnConnectionEvent += OnConnectionEvent;
            _manager.InitializeConnectionRequests();
        }

        private void OnConnectionEvent(ConnectionInformation connection)
        {
            connection.ConnectionChanged += OnConnectionChanged;
            PlusEnvironment.GetGame().GetClientManager().CreateAndStartClient(Convert.ToInt32(connection.GetConnectionId()), connection);
        }

        private void OnConnectionChanged(ConnectionInformation information, ConnectionState state)
        {
            if (state == ConnectionState.Closed)
            {
                CloseConnection(information);
            }
        }

        private void CloseConnection(ConnectionInformation connection)
        {
            try
            {
                connection.Dispose();
                PlusEnvironment.GetGame().GetClientManager().DisposeConnection(Convert.ToInt32( connection.GetConnectionId()));
            }
            catch (Exception e)
            {
                Core.ExceptionLogger.LogException(e);
            }
        }

        public void Destroy()
        {
            _manager.Destroy();
        }
    }
}