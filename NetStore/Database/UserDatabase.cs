using System;
using System.Collections.Generic;
using MySqlConnector;
using NetStore.Models;

namespace NetStore.Database;

public static class UserDatabase
{
    public static bool SetUser(User user)
    {
        string sqlCommand = "INSERT INTO User (name, surname, email, password, birthdate, role_id) " +
                            "VALUE (@Name, @Surname, @Email, @Password, @Birthdate, @RoleId)";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@Surname", user.Surname);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@Birthdate", user.Birthdate);
                cmd.Parameters.AddWithValue("@RoleId", (int)user.RoleId);

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
    
    public static User? GetUser(string email, string password)
    {
        string sqlCommand = "SELECT user_id, name, surname, email, password, birthdate, role_id " +
                            "FROM User " +
                            "WHERE email = @Email AND password = @Password " +
                            "LIMIT 1";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);
                
                dbConnection.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User()
                        {
                            UserId = reader.GetInt32("user_id"),
                            Name = reader.GetString("name"),
                            Surname = reader.GetString("surname"),
                            Email = reader.GetString("email"),
                            Password = reader.GetString("password"),
                            Birthdate = reader.GetDateTimeOffset("birthdate"),
                            RoleId = (UserRoleEnum)reader.GetInt32("role_id")
                        };
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return null;
    }
    
    public static bool ChangeRoleUser(User user, UserRoleEnum userRole)
    {
        string sqlCommand = "UPDATE User " +
                            "SET User.role_id = @UserRole " +
                            "WHERE user_id = @UserId";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@UserRole", (int)userRole);
                cmd.Parameters.AddWithValue("@UserId", user.UserId);

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
    
    public static bool ChangeUser(User user)
    {
        string sqlCommand = "UPDATE User " +
                            "SET name = @Name, surname = @Surname, email = @Email, " +
                            "password = @Password, birthdate = @Birthdate " +
                            "WHERE user_id = @UserId";
        
        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@Surname", user.Surname);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@Birthdate", user.Birthdate);
                cmd.Parameters.AddWithValue("@UserId", user.UserId);

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
    
    public static bool? IsEmailUse(string email)
    {
        string sqlCommand = "SELECT COUNT(User.email) " +
                            "FROM User " +
                            "WHERE User.email = @Email";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sqlCommand, dbConnection);
                cmd.Parameters.AddWithValue("@Email", email);

                dbConnection.Open();

                int countEmail = Convert.ToInt32(cmd.ExecuteScalar());
                return countEmail > 0;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public enum UserFieldEnum
    {
        Name,
        Surname,
        Email,
        Role,
        Birthdate
    }
    
    public static MySqlCommand GetQuerySelect(string? searchQuery = null, UserRoleEnum? roleEnum = null, 
        DateTimeOffset? birthdateFrom = null, DateTimeOffset? birthdateTo = null, UserFieldEnum? orderBy = null)
    {
        string sqlCommand = "SELECT user_id, name, surname, email, role_id, birthdate FROM User WHERE 1=1";

        if (!string.IsNullOrEmpty(searchQuery))
            sqlCommand += " AND (name LIKE @SearchQuery OR surname LIKE @SearchQuery OR email LIKE @SearchQuery)";
        if (roleEnum != null)
            sqlCommand += " AND role_id = @Role";
        if (birthdateFrom != null)
            sqlCommand += " AND birthdate >= @BirthdateFrom";
        if (birthdateTo != null)
            sqlCommand += " AND birthdate <= @BirthdateTo";

        sqlCommand += orderBy switch
        {
            UserFieldEnum.Name => " ORDER BY name",
            UserFieldEnum.Surname => " ORDER BY surname",
            UserFieldEnum.Email => " ORDER BY email",
            UserFieldEnum.Role => " ORDER BY role_id",
            UserFieldEnum.Birthdate => " ORDER BY birthdate",
            _ => " ORDER BY surname"
        };

        MySqlCommand cmd = new MySqlCommand(sqlCommand);
        if (!string.IsNullOrEmpty(searchQuery))
            cmd.Parameters.Add("@SearchQuery", MySqlDbType.VarChar).Value = "%" + searchQuery + "%";
        if (roleEnum != null)
            cmd.Parameters.AddWithValue("@Role", (int)roleEnum);
        if (birthdateFrom != null)
            cmd.Parameters.AddWithValue("@BirthdateFrom", birthdateFrom.Value.ToString("yyyy-MM-dd"));
        if (birthdateTo != null)
            cmd.Parameters.AddWithValue("@BirthdateTO", birthdateTo.Value.ToString("yyyy-MM-dd"));
        
        return cmd;
    }
    
    public static List<User> GetUsers(int pageNumber, int pageSize, MySqlCommand querySelect)
    {
        int offset = (pageNumber - 1) * pageSize;

        querySelect .CommandText = $"SELECT user_id, name, surname, email, role_id, birthdate FROM ({querySelect.CommandText}) qs " +
                             "LIMIT @PageSize " +
                             "OFFSET @Offset";

        List<User> users = new List<User>();

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
                        var user = new User()
                        {
                            UserId = reader.GetInt32("user_id"),
                            Name = reader.GetString("name"),
                            Surname = reader.GetString("surname"),
                            Email = reader.GetString("email"),
                            Birthdate = reader.GetDateTime("birthdate"),
                            RoleId = (UserRoleEnum)reader.GetInt32("role_id")
                        };

                        users.Add(user);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return users;
    }
    
    public static int GetTotalUsers(MySqlCommand querySelect)
    {
        querySelect .CommandText = $"SELECT COUNT(*) FROM ({querySelect.CommandText}) qs";

        try
        {
            using (MySqlConnection dbConnection = new MySqlConnection(NetStore.Config.ConnectionStringBuilder.ConnectionString))
            {
                querySelect.Connection = dbConnection;

                dbConnection.Open();

                int totalCount = Convert.ToInt32(querySelect.ExecuteScalar());
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