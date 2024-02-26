using System;
using System.Collections.Generic;
using System.Text;
using MySqlConnector;
using NetStore.Models;

namespace NetStore.Database;

public static class ProductManufacturerDatabase
{
    public static bool SetProductManufacturer(ProductManufacturer manufacturer)
    {
        string sqlCommand = "INSERT INTO ProductManufacturer (name) " +
                            "VALUES (@Name)";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@Name", manufacturer.Name);

                int rowsInserted = cmd.ExecuteNonQuery();
                return rowsInserted > 0;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
    
    public static bool ChangeProductManufacturer(ProductManufacturer manufacturer)
    {
        string sqlCommand = "UPDATE Product " +
                            "SET name = @Name " +
                            "WHERE manufacturer_id = @ManufacturerId";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@Name", manufacturer.Name);
                cmd.Parameters.AddWithValue("@ManufacturerId", manufacturer.ManufacturerId);

                int rowsUpdated = cmd.ExecuteNonQuery();
                return rowsUpdated > 0;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
    
    public static string GetQuerySelect(string? searchQuery = null)
    {
        string sqlCommand = "SELECT manufacturer_id, name FROM ProductManufacturer WHERE 1=1";

        if (!string.IsNullOrEmpty(searchQuery))
            sqlCommand += " AND (@SearchQuery LIKE name)";

        sqlCommand += " ORDER BY name";
        
        MySqlCommand cmd = new MySqlCommand(sqlCommand);
        if (!string.IsNullOrEmpty(searchQuery))
            cmd.Parameters.AddWithValue("@SearchQuery", searchQuery);
        
        return cmd.CommandText;
    }
    
    public static List<ProductManufacturer> GetProductManufacturers(int pageNumber, int pageSize, string querySelect)
    {
        int offset = (pageNumber - 1) * pageSize;

        string sqlCommand = "SELECT manufacturer_id, name FROM @QuerySelect " +
                            "LIMIT @PageSize OFFSET @Offset";

        List<ProductManufacturer> manufacturers = new List<ProductManufacturer>();

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@QuerySelect", querySelect);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                cmd.Parameters.AddWithValue("@Offset", offset);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var manufacturer = new ProductManufacturer()
                        {
                            ManufacturerId = reader.GetInt32("manufacturer_id"),
                            Name = reader.GetString("name")
                        };

                        manufacturers.Add(manufacturer);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return manufacturers;
    }
    
    public static int GetTotalManufacturers(string querySelect)
    {
        string sqlCommand = "SELECT COUNT(*) FROM @QuerySelect";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@QuerySelect", querySelect);
                int total = Convert.ToInt32(cmd.ExecuteScalar());
                return total;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return -1;
        }
    }
}