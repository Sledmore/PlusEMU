using System;
using MySql.Data.MySqlClient;
using Plus.Core;
using Plus.Database.Interfaces;

namespace Plus.Database
{
    public sealed class DatabaseManager
    {
        private readonly string _connectionStr;

        public DatabaseManager(string connectionString)
        {
            _connectionStr = connectionString;
        }

        public bool IsConnected()
        {
            try
            {
                MySqlConnection con = new MySqlConnection(_connectionStr);
                con.Open();
                MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT 1+1";
                cmd.ExecuteNonQuery();

                cmd.Dispose();
                con.Close();
            }
            catch (MySqlException)
            {
                return false;
            }

            return true;
        }

        public IQueryAdapter GetQueryReactor()
        {
            try
            {
                IDatabaseClient dbConnection = new DatabaseConnection(_connectionStr);
              
                dbConnection.Connect();

                return dbConnection.GetQueryReactor();
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
                return null;
            }
        }
    }
}