using System;
using System.Collections.Generic;
using System.Text;
using MySqlConnector;
using NetStore.Models;

namespace NetStore.Database;

public static class CartItemDatabase
{
    public static bool SetCartItem(CartItem cartItem)
    {
        string sqlCommand = "INSERT INTO CartItem (quantity, user_id, product_id) " +
                            "VALUES (@Quantity, @UserId, @ProductId)";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@Quantity", cartItem.Quantity);
                cmd.Parameters.AddWithValue("@UserId", cartItem.UserId);
                cmd.Parameters.AddWithValue("@ProductId", cartItem.ProductId);

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
    
    public static bool ChangeCartItem(CartItem updatedCartItem)
    {
        string sqlCommand = "UPDATE CartItem " +
                            "SET quantity = @Quantity, user_id = @UserId, product_id = @ProductId " +
                            "WHERE item_id = @ItemId";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@Quantity", updatedCartItem.Quantity);
                cmd.Parameters.AddWithValue("@UserId", updatedCartItem.UserId);
                cmd.Parameters.AddWithValue("@ProductId", updatedCartItem.ProductId);
                cmd.Parameters.AddWithValue("@ItemId", updatedCartItem.ItemId);

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

    public enum CartItemFieldEnum
    {
        Quantity,
        UserId,
        ProductId,
    }
    
    public static string GetQuerySelect(User? user = null, Product? product = null, CartItemFieldEnum? orderBy = null)
    {
        string sqlCommand = "SELECT item_id, quantity, user_id, product_id FROM CartItem WHERE 1=1";
        
        if (user != null)
            sqlCommand += " AND user_id = @UserId";
        if (product != null)
            sqlCommand += " AND product_id >= @ProductId";

        sqlCommand += orderBy switch
        {
            CartItemFieldEnum.Quantity => " ORDER BY quantity",
            CartItemFieldEnum.UserId => " ORDER BY user_id",
            CartItemFieldEnum.ProductId => " ORDER BY product_id",
            _ => " ORDER BY product_id"
        };

        MySqlCommand cmd = new MySqlCommand(sqlCommand);
        if (user != null)
            cmd.Parameters.AddWithValue("@UserId", user.UserId);
        if (product != null)
            cmd.Parameters.AddWithValue("@ProductId", product.ProductId);
        
        return cmd.CommandText;
    }
    
    public static List<CartItem> GetCartItems(int pageNumber, int pageSize, string querySelect)
    {
        int offset = (pageNumber - 1) * pageSize;

        string sqlCommand = "SELECT item_id, quantity, user_id, product_id FROM @QuerySelect " +
                            "LIMIT @PageSize OFFSET @Offset";

        List<CartItem> cartItems = new List<CartItem>();

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
                        CartItem cartItem = new CartItem()
                        {
                            ItemId = reader.GetInt32("item_id"),
                            Quantity = reader.GetInt32("quantity"),
                            UserId = reader.GetInt32("user_id"),
                            ProductId = reader.GetInt32("product_id")
                        };

                        cartItems.Add(cartItem);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return cartItems;
    }
    
    public static int GetTotalCartItems(string querySelect)
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
    
    public static bool DeleteCartItem(CartItem item)
    {
        string sqlCommand = "DELETE FROM CartItem " +
                            "WHERE item_id = @ItemId";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@ItemId", item.ItemId);

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