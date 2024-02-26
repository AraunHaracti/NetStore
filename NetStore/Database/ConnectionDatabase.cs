using System;
using MySqlConnector;

namespace NetStore.Database;

public static class ConnectionDatabase
{
    public static bool CheckConnection()
    {
        try
        {
            using MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString);
            
            dbConnection.Open();

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        
    }
}