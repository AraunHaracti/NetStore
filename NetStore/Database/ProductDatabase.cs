using System;
using System.Collections.Generic;
using System.Text;
using MySqlConnector;
using NetStore.Models;

namespace NetStore.Database;

public class ProductDatabase
{
    public static bool SetProduct(Product product)
    {
        string sqlCommand = "INSERT INTO Product (name, description, price, quantity, category_id, manufacturer_id) " +
                            "VALUES (@Name, @Description, @Price, @Quantity, @CategoryId, @ManufacturerId)";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@Name", product.Name);
                cmd.Parameters.AddWithValue("@Description", product.Description);
                cmd.Parameters.AddWithValue("@Price", product.Price);
                cmd.Parameters.AddWithValue("@Quantity", product.Quantity);
                cmd.Parameters.AddWithValue("@CategoryId", product.CategoryId);
                cmd.Parameters.AddWithValue("@ManufacturerId", product.ManufacturerId);

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

    public static bool ChangeProduct(Product updatedProduct)
    {
        string sqlCommand = "UPDATE Product " +
                            "SET name = @Name, description = @Description, price = @Price, " +
                            "quantity = @Quantity, category_id = @CategoryId, manufacturer_id = @ManufacturerId " +
                            "WHERE product_id = @ProductId";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@Name", updatedProduct.Name);
                cmd.Parameters.AddWithValue("@Description", updatedProduct.Description);
                cmd.Parameters.AddWithValue("@Price", updatedProduct.Price);
                cmd.Parameters.AddWithValue("@Quantity", updatedProduct.Quantity);
                cmd.Parameters.AddWithValue("@CategoryId", updatedProduct.CategoryId);
                cmd.Parameters.AddWithValue("@ManufacturerId", updatedProduct.ManufacturerId);
                cmd.Parameters.AddWithValue("@ProductId", updatedProduct.ProductId);

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

    public enum ProductFieldEnum
    {
        Name, 
        Description, 
        Price, 
        Quantity, 
        Category, 
        Manufacturer
    }
    
    public static MySqlCommand GetQuerySelect(string? searchQuery = null, 
        ProductManufacturer? manufacturer = null, 
        ProductCategory? category = null, 
        decimal? priceFrom = null, decimal? priceTo = null, 
        int? quantityFrom = null, int? quantityTo = null, 
        ProductFieldEnum? orderBy = null)
    {
        string sqlCommand = "SELECT product_id, name, description, price, quantity, category_id, manufacturer_id FROM Product WHERE 1=1";

        if (!string.IsNullOrWhiteSpace(searchQuery))
            sqlCommand += " AND ((name LIKE @SearchQuery) OR (description LIKE @SearchQuery))";
        if (manufacturer != null)
            sqlCommand += " AND manufacturer_id = @ManufacturerId";
        if (category != null)
            sqlCommand += " AND category_id = @CategoryId";
        if (priceFrom != null)
            sqlCommand += " AND price >= @PriceFrom";
        if (priceTo != null)
            sqlCommand += " AND price <= @PriceTo";
        if (quantityFrom != null)
            sqlCommand += " AND quantity >= @QuantityFrom";
        if (quantityTo != null)
            sqlCommand += " AND quantity <= @QuantityTo";

        sqlCommand += orderBy switch
        {
            ProductFieldEnum.Name => " ORDER BY name",
            ProductFieldEnum.Description => " ORDER BY description",
            ProductFieldEnum.Price => " ORDER BY price",
            ProductFieldEnum.Quantity => " ORDER BY quantity",
            ProductFieldEnum.Category => " ORDER BY category_id",
            ProductFieldEnum.Manufacturer => " ORDER BY manufacturer_id",
            _ => " ORDER BY name"
        };

        MySqlCommand cmd = new MySqlCommand(sqlCommand);
        if (!string.IsNullOrWhiteSpace(searchQuery))
            cmd.Parameters.Add("@SearchQuery", MySqlDbType.VarChar).Value = "%" + searchQuery + "%";
        if (manufacturer != null)
            cmd.Parameters.AddWithValue("@ManufacturerId", manufacturer.ManufacturerId);
        if (category != null)
            cmd.Parameters.AddWithValue("@CategoryId", category.CategoryId);
        if (priceFrom != null)
            cmd.Parameters.AddWithValue("@PriceFrom", priceFrom);
        if (priceTo != null)
            cmd.Parameters.AddWithValue("@PriceTo", priceTo);
        if (quantityFrom != null)
            cmd.Parameters.AddWithValue("@QuantityFrom", quantityFrom);
        if (quantityTo != null)
            cmd.Parameters.AddWithValue("@QuantityTo", quantityTo);
        
        return cmd;
    }
    
    public static List<Product> GetProducts(int pageNumber, int pageSize, MySqlCommand querySelect)
    {
        int offset = (pageNumber - 1) * pageSize;

        querySelect.CommandText = $"SELECT product_id, name, description, price, quantity, category_id, manufacturer_id FROM ({querySelect.CommandText}) qs " +
                                         "LIMIT @PageSize OFFSET @Offset";

        List<Product> products = new List<Product>();

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
                        Product product = new Product()
                        {
                            ProductId = reader.GetInt32("product_id"),
                            Name = reader.GetString("name"),
                            Description = reader.GetString("description"),
                            Price = reader.GetDecimal("price"),
                            Quantity = reader.GetInt32("quantity"),
                            CategoryId = reader.IsDBNull(reader.GetOrdinal("category_id")) ? null : reader.GetInt32("category_id"),
                            ManufacturerId = reader.IsDBNull(reader.GetOrdinal("manufacturer_id")) ? null : reader.GetInt32("manufacturer_id")
                        };

                        products.Add(product);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return products;
    }
    
    public static int GetTotalProducts(MySqlCommand querySelect)
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