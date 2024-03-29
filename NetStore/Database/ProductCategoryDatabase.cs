﻿using System;
using System.Collections.Generic;
using MySqlConnector;
using NetStore.Models;

namespace NetStore.Database;

public static class ProductCategoryDatabase
{
    public static bool SetProductCategory(ProductCategory category)
    {
        string sqlCommand = "INSERT INTO ProductCategory (name) " +
                            "VALUES (@Name)";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@Name", category.Name);
                
                dbConnection.Open();

                int rowsInserted = cmd.ExecuteNonQuery();
                return rowsInserted > 0;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e); // TODO: ПЕРЕПИСАТЬ ЛОГИРОВАНИЕ ВО ВСЕЙ ПРОГРАММЕ!!!
            return false;
        }
    }

    public static bool ChangeProductCategory(ProductCategory category)
    {
        string sqlCommand = "UPDATE ProductCategory " +
                            "SET name = @Name " +
                            "WHERE category_id = @CategoryId";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@Name", category.Name);
                cmd.Parameters.AddWithValue("@CategoryId", category.CategoryId);

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
        string sqlCommand = "SELECT category_id, name FROM ProductCategory WHERE 1=1";

        if (!string.IsNullOrEmpty(searchQuery))
            sqlCommand += " AND (name LIKE @SearchQuery)";

        sqlCommand += " ORDER BY name";
        
        MySqlCommand cmd = new MySqlCommand(sqlCommand);
        if (!string.IsNullOrEmpty(searchQuery))
            cmd.Parameters.Add("@SearchQuery", MySqlDbType.VarChar).Value = "%" + searchQuery + "%";
        
        return cmd;
    }

    public static List<ProductCategory> GetProductCategories(int pageNumber, int pageSize, MySqlCommand querySelect)
    {
        int offset = (pageNumber - 1) * pageSize;

        querySelect.CommandText = $"SELECT category_id, name FROM ({querySelect.CommandText}) qs LIMIT @PageSize OFFSET @Offset";

        List<ProductCategory> categories = new List<ProductCategory>();

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
                        var category = new ProductCategory()
                        {
                            CategoryId = reader.GetInt32("category_id"),
                            Name = reader.GetString("name")
                        };

                        categories.Add(category);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return categories;
    }

    public static int GetTotalProductCategories(MySqlCommand querySelect)
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