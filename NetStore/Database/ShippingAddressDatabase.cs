using System;
using System.Collections.Generic;
using System.Text;
using MySqlConnector;
using NetStore.Models;

namespace NetStore.Database;

public static class ShippingAddressDatabase
{
    public static bool SetShippingAddress(ShippingAddress shippingAddress)
    {
        string sqlCommand = "INSERT INTO ShippingAddress (city, country, postal_code, street, house)" +
                            "VALUES (@City, @Country, @PostalCode, @Street, @House)";

        try // TODO: Переделать БД по моделям
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                dbConnection.Open();

                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@Street", shippingAddress.Street);
                cmd.Parameters.AddWithValue("@House", shippingAddress.House);
                cmd.Parameters.AddWithValue("@City", shippingAddress.City);
                cmd.Parameters.AddWithValue("@Country", shippingAddress.Country);
                cmd.Parameters.AddWithValue("@PostalCode", shippingAddress.PostalCode);

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
    
    public static bool ChangeShippingAddress(ShippingAddress updatedShippingAddress)
    {
        string sqlCommand = @"UPDATE ShippingAddress 
                              SET city = @City, country = @Country, postal_code = @PostalCode, street = @Street, house = @House
                              WHERE address_id = @AddressId";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                dbConnection.Open();

                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@Street", updatedShippingAddress.Street);
                cmd.Parameters.AddWithValue("@House", updatedShippingAddress.House);
                cmd.Parameters.AddWithValue("@City", updatedShippingAddress.City);
                cmd.Parameters.AddWithValue("@Country", updatedShippingAddress.Country);
                cmd.Parameters.AddWithValue("@PostalCode", updatedShippingAddress.PostalCode);
                cmd.Parameters.AddWithValue("@AddressId", updatedShippingAddress.AddressId);

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
    
    public enum ShippingAddressFieldEnum
    {
        City,
        Country,
        PostalCode,
        Street,
        House
    }
    
    public static string GetQuerySelect(string? searchQuery,ShippingAddressFieldEnum? orderBy)
    {
        string sqlCommand = "SELECT address_id, city, country, postal_code, street, house FROM ShippingAddress WHERE 1=1";

        if (!string.IsNullOrEmpty(searchQuery))
            sqlCommand += " AND (@SearchQuery LIKE name OR @SearchQuery LIKE surname OR @SearchQuery LIKE email)";

        sqlCommand += orderBy switch
        {
            ShippingAddressFieldEnum.City => " ORDER BY city",
            ShippingAddressFieldEnum.Country => " ORDER BY country",
            ShippingAddressFieldEnum.PostalCode => " ORDER BY postal_code",
            ShippingAddressFieldEnum.Street => " ORDER BY street",
            ShippingAddressFieldEnum.House => " ORDER BY house",
            _ => " ORDER BY country"
        };

        MySqlCommand cmd = new MySqlCommand(sqlCommand);
        if (!string.IsNullOrEmpty(searchQuery))
            cmd.Parameters.AddWithValue("@SearchQuery", searchQuery);

        return cmd.CommandText;
    }
    
    public static List<ShippingAddress> GetShippingAddresses(int pageNumber, int pageSize, string querySelect)
    {
        int offset = (pageNumber - 1) * pageSize;

        string sqlCommand = @"SELECT * FROM @QuerySelect 
                              ORDER BY address_id 
                              LIMIT @PageSize OFFSET @Offset";

        List<ShippingAddress> shippingAddresses = new List<ShippingAddress>();

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                dbConnection.Open();

                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@QuerySelect", querySelect);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                cmd.Parameters.AddWithValue("@Offset", offset);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ShippingAddress address = new ShippingAddress()
                        {
                            AddressId = reader.GetInt32("address_id"),
                            City = reader.GetString("city"),
                            Country = reader.GetString("country"),
                            PostalCode = reader.GetString("postal_code"),
                            Street = reader.GetString("street"),
                            House = reader.GetString("house")
                        };

                        shippingAddresses.Add(address);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return shippingAddresses;
    }
    
    public static int GetTotalShippingAddresses(string querySelect)
    {
        string sqlCommand = "SELECT COUNT(*) FROM @QuerySelect";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@QuerySelect", querySelect);
                int totalCount = Convert.ToInt32(cmd.ExecuteScalar());
                return totalCount;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return -1;
        }
    }
}