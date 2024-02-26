using System;
using System.Collections.Generic;
using System.Text;
using MySqlConnector;
using NetStore.Models;

namespace NetStore.Database;

public static class OrderDatabase
{
    public static bool SetOrder(Order order)
    {
        string sqlCommand = @"INSERT INTO `Order` (created_at, user_id, shipping_address_id, status_id)
                              VALUES (@CreatedAt, @UserId, @ShippingAddressId, @StatusId)";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@CreatedAt", DateTimeOffset.Now);
                cmd.Parameters.AddWithValue("@UserId", order.UserId);
                cmd.Parameters.AddWithValue("@ShippingAddressId", order.ShippingAddressId);
                cmd.Parameters.AddWithValue("@StatusId", (int)order.StatusId);

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
    
    public static bool ChangeOrderStatus(Order order, OrderStatusEnum orderStatus)
    {
        string sqlCommand = @"UPDATE `Order` 
                              SET status_id = @StatusId
                              WHERE order_id = @OrderId";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@StatusId", (int)orderStatus);
                cmd.Parameters.AddWithValue("@OrderId", order.OrderId);

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
    
    public static bool ChangeOrderShippingAddress(Order order, ShippingAddress shippingAddress)
    {
        string sqlCommand = @"UPDATE `Order` 
                              SET shipping_address_id = @ShippingAddress
                              WHERE order_id = @OrderId";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                dbConnection.Open();

                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@ShippingAddress", shippingAddress.AddressId);
                cmd.Parameters.AddWithValue("@OrderId", order.OrderId);

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

    public enum OrderFieldEnum
    {
        CreatedAt,
        User,
        ShippingAddress,
        Status,
    }
    
    public static string GetQuerySelect(
        User? user, ShippingAddress? address,
        DateTimeOffset? createAtFrom = null, 
        DateTimeOffset? createAtTo = null, 
        OrderFieldEnum? orderBy = null)
    {
        string sqlCommand = "SELECT order_id, create_at, user_id, address_id, status_id FROM Order WHERE 1=1";
        
        if (user != null)
            sqlCommand += " AND user_id = @UserId";
        if (address != null)
            sqlCommand += " AND address_id = @AddressId";
        if (createAtFrom != null)
            sqlCommand += " AND create_at >= @CreateAtFrom";
        if (createAtTo != null)
            sqlCommand += " AND create_at <= @CreateAtTo";

        sqlCommand += orderBy switch
        {
            OrderFieldEnum.CreatedAt => " ORDER BY created_at",
            OrderFieldEnum.User => " ORDER BY user_id",
            OrderFieldEnum.ShippingAddress => " ORDER BY address_id",
            OrderFieldEnum.Status => " ORDER BY status_id",
            _ => " ORDER BY created_at"
        };

        MySqlCommand cmd = new MySqlCommand(sqlCommand);
        if (user != null)
            cmd.Parameters.AddWithValue("@UserId", user.UserId);
        if (address != null)
            cmd.Parameters.AddWithValue("@AddressId", address.AddressId);
        if (createAtFrom != null)
            cmd.Parameters.AddWithValue("@CreateAtFrom", createAtFrom.Value.ToString("yyyy-MM-dd"));
        if (createAtTo != null)
            cmd.Parameters.AddWithValue("@CreateAtTo", createAtTo.Value.ToString("yyyy-MM-dd"));
        
        return cmd.CommandText;
    }

    public static List<Order> GetOrders(int pageNumber, int pageSize, string querySelect)
    {
        int offset = (pageNumber - 1) * pageSize;

        string sqlCommand = "SELECT * FROM @QuerySelect " +
                            "LIMIT @PageSize OFFSET @Offset";

        List<Order> orders = new List<Order>();

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
                    while (reader.Read())
                    {
                        Order order = new Order()
                        {
                            OrderId = reader.GetInt32("order_id"),
                            CreatedAt = reader.GetDateTime("created_at"),
                            UserId = reader.GetInt32("user_id"),
                            ShippingAddressId = reader.GetInt32("shipping_address_id"),
                            StatusId = (OrderStatusEnum)reader.GetInt32("status_id")
                        };

                        orders.Add(order);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return orders;
    }
    
    public static int GetTotalOrders(string querySelect)
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
    
    public static bool DeleteOrder(Order order)
    {
        string sqlCommand = @"DELETE FROM `Order`
                              WHERE order_id = @OrderId";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@OrderId", order.OrderId);

                int rowsDeleted = cmd.ExecuteNonQuery();
                return rowsDeleted > 0;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}