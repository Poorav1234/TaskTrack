using Npgsql;
using Repositories;
using Repositories.Models;

public class UserRepository : IUserInterface
{
    private readonly NpgsqlConnection con;
    public UserRepository(NpgsqlConnection connection)
    {
        con = connection;

        try
        {
            con.Open();
            Console.WriteLine("Database connection successfully done");
            con.Close();
        }
        catch
        {
            Console.WriteLine("Database connection failed");
        }
    }

    public async Task<int> Register(t_user user)
    {
        int status = 0;
        try
        {
            await con.CloseAsync();
            NpgsqlCommand cmd = new NpgsqlCommand(@"SELECT * FROM t_user WHERE email = @email", con);
            cmd.Parameters.AddWithValue("@email", user.email);
            await con.OpenAsync();
            using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                if (reader.HasRows)
                {
                    await con.CloseAsync();
                    status = 0;
                }
                else
                {
                    await con.CloseAsync();
                    NpgsqlCommand cmd1 = new NpgsqlCommand(@"INSERT INTO t_user (username, email, mobile, gender, city, password) VALUES (@username, @email, @mobile, @gender, @city, @password)", con);
                    cmd1.Parameters.AddWithValue("@username", user.username);
                    cmd1.Parameters.AddWithValue("@email", user.email);
                    cmd1.Parameters.AddWithValue("@mobile", user.mobile);
                    cmd1.Parameters.AddWithValue("@gender", user.gender);
                    cmd1.Parameters.AddWithValue("@city", user.city);
                    cmd1.Parameters.AddWithValue("@password", user.password);
                    await con.OpenAsync();
                    status = await cmd1.ExecuteNonQueryAsync();
                    status = 1;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in Register");
            Console.WriteLine(ex.Message);
            return -1;
        }
        finally
        {
            await con.CloseAsync();
        }
        return status;
    }

    public async Task<t_user> Login(vm_Login login)
    {
        t_user user = new t_user();
        try
        {
            await con.CloseAsync();
            NpgsqlCommand cmd = new NpgsqlCommand(@"SELECT * FROM t_user WHERE email = @email AND password = @password", con);
            cmd.Parameters.AddWithValue("@email", login.email);
            cmd.Parameters.AddWithValue("@password", login.password);
            await con.OpenAsync();
            using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                if (reader.HasRows)
                {
                    await reader.ReadAsync();
                    user.user_id = (int)reader["user_id"];
                    user.username = (string)reader["username"];
                    user.email = (string)reader["email"];
                    user.mobile = (string)reader["mobile"];
                    user.gender = (string)reader["gender"];
                    user.city = (string)reader["city"];
                    user.password = (string)reader["password"];
                }
                else
                {
                    user = null;
                }
            }
        }
        catch (Exception)
        {
            await con.CloseAsync();
            Console.WriteLine("Error in Login");
        }
        finally
        {
            await con.CloseAsync();
        }
        return user;
    }

    public async Task<t_user> GetUser(string user_id)
    {
        t_user user = new t_user();
        try
        {
            await con.CloseAsync();
            NpgsqlCommand cmd = new NpgsqlCommand(@"SELECT * FROM t_user WHERE user_id = @user_id", con);
            cmd.Parameters.AddWithValue("@user_id", user_id);
            await con.OpenAsync();
            using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                if (reader.HasRows)
                {
                    await reader.ReadAsync();
                    user.user_id = (int)reader["user_id"];
                    user.username = (string)reader["username"];
                    user.email = (string)reader["email"];
                    user.mobile = (string)reader["mobile"];
                    user.gender = (string)reader["gender"];
                    user.city = (string)reader["city"];
                    user.password = (string)reader["password"];
                }
                else
                {
                    user = null;
                }
            }
        }
        catch (Exception)
        {
            await con.CloseAsync();
            Console.WriteLine("Error in GetUser");
        }
        finally
        {
            await con.CloseAsync();
        }
        return user;
    }
}
