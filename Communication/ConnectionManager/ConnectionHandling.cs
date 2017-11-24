using System;
using Plus.Communication.ConnectionManager;
using Plus.Communication;

namespace Plus.Communication.ConnectionManager
{
    public class ConnectionHandling
    {
        private readonly SocketManager manager;

        public ConnectionHandling(int port, int maxConnections, int connectionsPerIP, bool enabeNagles)
        {
            manager = new SocketManager();
            manager.init(port, maxConnections, connectionsPerIP, new InitialPacketParser(), !enabeNagles);
        }

        public void init()
        {
            manager.connectionEvent += manager_connectionEvent;
            manager.initializeConnectionRequests();
        }

        private void manager_connectionEvent(ConnectionInformation connection)
        {
            connection.connectionChanged += connectionChanged;
            PlusEnvironment.GetGame().GetClientManager().CreateAndStartClient(Convert.ToInt32(connection.getConnectionID()), connection);
        }

        private void connectionChanged(ConnectionInformation information, ConnectionState state)
        {
            if (state == ConnectionState.CLOSED)
            {
                CloseConnection(information);
            }
        }

        private void CloseConnection(ConnectionInformation Connection)
        {
            try
            {
                Connection.Dispose();
                PlusEnvironment.GetGame().GetClientManager().DisposeConnection(Convert.ToInt32( Connection.getConnectionID()));
            }
            catch (Exception e)
            {
                Core.ExceptionLogger.LogException(e);
            }
        }

        public void Destroy()
        {
            manager.destroy();
        }
    }
}