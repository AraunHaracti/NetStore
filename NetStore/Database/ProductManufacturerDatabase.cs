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

                dbConnection.Open();
                
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
        string sqlCommand = "UPDATE ProductManufacturer " +
                            "SET name = @Name " +
                            "WHERE manufacturer_id = @ManufacturerId";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@Name", manufacturer.Name);
                cmd.Parameters.AddWithValue("@ManufacturerId", manufacturer.ManufacturerId);

                dbConnection.Open();

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
    
    public static MySqlCommand GetQuerySelect(string? searchQuery = null)
    {
        string sqlCommand = "SELECT manufacturer_id, name FROM ProductManufacturer WHERE 1=1";

        if (!string.IsNullOrEmpty(searchQuery))
            sqlCommand += " AND (name LIKE @SearchQuery)";

        sqlCommand += " ORDER BY name";
        
        MySqlCommand cmd = new MySqlCommand(sqlCommand);
        if (!string.IsNullOrEmpty(searchQuery))
            cmd.Parameters.Add("@SearchQuery", MySqlDbType.VarChar).Value = "%" + searchQuery + "%";
        
        return cmd;
    }
    
    public static List<ProductManufacturer> GetProductManufacturers(int pageNumber, int pageSize, MySqlCommand querySelect)
    {
        int offset = (pageNumber - 1) * pageSize;

        querySelect.CommandText = $"SELECT manufacturer_id, name FROM ({querySelect.CommandText}) qs " +
                                  "LIMIT @PageSize OFFSET @Offset";

        List<ProductManufacturer> manufacturers = new List<ProductManufacturer>();

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                querySelect.Connection = dbConnection;
                querySelect.Parameters.AddWithValue("@PageSize", pageSize);
                querySelect.Parameters.AddWithValue("@Offset", offset);
                
                dbConnection.Open();

                using (MySqlDataReader reader = querySelect.ExecuteReader())
                {
                    while (reader.Read())
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
    
    public static int GetTotalProductManufacturers(MySqlCommand querySelect)
    {
        querySelect.CommandText = $"SELECT COUNT(*) FROM ({querySelect.CommandText}) qs";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                querySelect.Connection = dbConnection;
                
                dbConnection.Open();
                
                int total = Convert.ToInt32(querySelect.ExecuteScalar());
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