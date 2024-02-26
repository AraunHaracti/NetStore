using System;
using System.Collections.Generic;
using MySqlConnector;
using NetStore.Models;

namespace NetStore.Database;

public static class OrderItemDatabase
{
    public static bool SetOrderItem(OrderItem orderItem)
    {
        string sqlCommand = "INSERT INTO OrderItem (quantity, price_per_unit, order_id, product_id) " +
                            "VALUES (@Quantity, @PricePerUnit, @OrderId, @ProductId)";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                dbConnection.Open();

                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@Quantity", orderItem.Quantity);
                cmd.Parameters.AddWithValue("@PricePerUnit", orderItem.PricePerUnit);
                cmd.Parameters.AddWithValue("@OrderId", orderItem.OrderId);
                cmd.Parameters.AddWithValue("@ProductId", orderItem.ProductId);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public enum OrderItemFieldEnum
    {
        Quantity,
        PricePerUnit, 
        Order,
        Product 
    }
    
    public static string GetQuerySelect(int? quantityFrom = null, int? quantityTo = null, 
        decimal? pricePerUnitFrom = null, decimal? pricePerUnitTo = null,
        Order? order = null, Product? product = null, OrderItemFieldEnum? orderBy = null)
    {
        string sqlCommand = "SELECT item_id, quantity, price_per_unit, order_id, product_id FROM OrderItem WHERE 1=1";

        if (quantityFrom != null)
            sqlCommand += " AND quantity >= @QuantityFrom";
        if (quantityTo != null)
            sqlCommand += " AND quantity <= @QuantityTo";
        if (pricePerUnitFrom != null)
            sqlCommand += " AND price_per_unit >= @PricePerUnitFrom";
        if (pricePerUnitTo != null)
            sqlCommand += " AND price_per_unit <= @PricePerUnitTo";
        if (order != null)
            sqlCommand += " AND order_id = @OrderId";
        if (product != null)
            sqlCommand += " AND product_id = @ProductId";

        sqlCommand += orderBy switch
        {
            OrderItemFieldEnum.Quantity => " ORDER BY quantity",
            OrderItemFieldEnum.PricePerUnit => " ORDER BY price_per_unit",
            OrderItemFieldEnum.Order => " ORDER BY order_id",
            OrderItemFieldEnum.Product => " ORDER BY product_id",
            _ => " ORDER BY product_id"
        };

        MySqlCommand cmd = new MySqlCommand(sqlCommand);
        if (quantityFrom != null)
            cmd.Parameters.AddWithValue("@QuantityFrom", quantityFrom);
        if (quantityTo != null)
            cmd.Parameters.AddWithValue("@QuantityTo", quantityTo);
        if (pricePerUnitFrom != null)
            cmd.Parameters.AddWithValue("@PricePerUnitFrom", pricePerUnitFrom);
        if (pricePerUnitTo != null)
            cmd.Parameters.AddWithValue("@PricePerUnitTo", pricePerUnitTo);
        if (order != null)
            cmd.Parameters.AddWithValue("@OrderId", order.OrderId);
        if (product != null)
            cmd.Parameters.AddWithValue("@ProductId", product.ProductId);

        return cmd.CommandText;
    }
    
     public static List<OrderItem> GetOrderItems(int pageNumber, int pageSize, string querySelect)
    {
        int offset = (pageNumber - 1) * pageSize;

        string sqlCommand = "SELECT item_id, quantity, price_per_unit, order_id, product_id FROM @QuerySelect " +
                            "ORDER BY item_id " +
                            "LIMIT @PageSize OFFSET @Offset";

        List<OrderItem> orderItems = new List<OrderItem>();

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
                        OrderItem orderItem = new OrderItem()
                        {
                            ItemId = reader.GetInt32("item_id"),
                            Quantity = reader.GetInt32("quantity"),
                            PricePerUnit = reader.GetDecimal("price_per_unit"),
                            OrderId = reader.GetInt32("order_id"),
                            ProductId = reader.GetInt32("product_id")
                        };

                        orderItems.Add(orderItem);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return orderItems;
    }
     
    public static int GetTotalUsers(string querySelect)
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
    
    public static bool DeleteOrderItem(OrderItem item)
    {
        string sqlCommand = @"DELETE FROM OrderItem 
                              WHERE item_id = @ItemId";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                dbConnection.Open();

                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@ItemId", item.ItemId);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}