using Npgsql;
using Repositories;
using Repositories.Models;

public class UserRepository : IUserInterface
{
    private readonly NpgsqlConnection con;
    public UserRepository(NpgsqlConnection connection)
    {
        con = connection;
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

    public async Task<t_user> GetUser(int user_id)
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

    public async Task<int> UpdateProfile(t_user user)
    {
        try
        {
            await con.CloseAsync();

            NpgsqlCommand cmd = new NpgsqlCommand(@"UPDATE t_user
                                                    SET username = @username,
                                                        email = @email,
                                                        mobile = @mobile,
                                                        gender = @gender,
                                                        city = @city
                                                    WHERE user_id = @user_id", con);

            cmd.Parameters.AddWithValue("@user_id", user.user_id);
            cmd.Parameters.AddWithValue("@username", user.username);
            cmd.Parameters.AddWithValue("@email", user.email);
            cmd.Parameters.AddWithValue("@mobile", user.mobile);
            cmd.Parameters.AddWithValue("@gender", user.gender);
            cmd.Parameters.AddWithValue("@city", user.city);

            await con.OpenAsync();

            int result = await cmd.ExecuteNonQueryAsync();

            await con.CloseAsync();

            return result > 0 ? 1 : 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in UpdateProfile");
            Console.WriteLine(ex.Message);

            return -1;
        }
        finally
        {
            await con.CloseAsync();
        }
    }

    public async Task<int> ChangePassword(
    int user_id,
    vm_ChangePassword password)
    {
        try
        {
            string query = @"SELECT password
                         FROM t_user
                         WHERE user_id = @user_id";

            using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@user_id", user_id);

                await con.OpenAsync();

                object result = await cmd.ExecuteScalarAsync();

                await con.CloseAsync();

                if (result == null)
                {
                    return 0;
                }

                string oldPassword = result.ToString();

                if (oldPassword != password.old_password)
                {
                    return 0;
                }
            }

            string updateQuery = @"UPDATE t_user
                               SET password = @password
                               WHERE user_id = @user_id";

            using (NpgsqlCommand cmd = new NpgsqlCommand(updateQuery, con))
            {
                cmd.Parameters.AddWithValue(
                    "@user_id",
                    user_id
                );

                cmd.Parameters.AddWithValue(
                    "@password",
                    password.new_password
                );

                await con.OpenAsync();

                int result = await cmd.ExecuteNonQueryAsync();

                await con.CloseAsync();

                return result > 0 ? 1 : 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in ChangePassword");
            Console.WriteLine(ex.Message);

            if (con.State == System.Data.ConnectionState.Open)
            {
                await con.CloseAsync();
            }

            return -1;
        }
    }

    public async Task<List<t_user>> GetUsers()
    {
        List<t_user> users = new List<t_user>();

        try
        {
            string query = @"SELECT user_id, username, email,
                                mobile, gender, city, password
                         FROM t_user
                         ORDER BY user_id";

            using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
            {
                await con.OpenAsync();

                using (NpgsqlDataReader reader =
                       await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        t_user user = new t_user
                        {
                            user_id = Convert.ToInt32(reader["user_id"]),
                            username = reader["username"].ToString(),
                            email = reader["email"].ToString(),
                            mobile = reader["mobile"].ToString(),
                            gender = reader["gender"].ToString(),
                            city = reader["city"].ToString()
                        };

                        users.Add(user);
                    }
                }

                await con.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in GetUsers");
            Console.WriteLine(ex.Message);

            if (con.State == System.Data.ConnectionState.Open)
            {
                await con.CloseAsync();
            }
        }

        return users;
    }

    public async Task<int> GetTotalUsers()
    {
        try
        {
            string query = @"SELECT COUNT(*) FROM t_user";

            using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
            {
                await con.OpenAsync();

                int result = Convert.ToInt32(
                    await cmd.ExecuteScalarAsync()
                );

                await con.CloseAsync();

                return result;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in GetTotalUsers");
            Console.WriteLine(ex.Message);

            if (con.State == System.Data.ConnectionState.Open)
            {
                await con.CloseAsync();
            }

            return 0;
        }


    }

    public async Task<List<t_user>> SearchUsers(
    string username,
    string email,
    string mobile,
    string gender,
    string city)
    {
        List<t_user> users = new List<t_user>();

        try
        {
            string query = @"SELECT user_id, username, email,
                                mobile, gender, city
                         FROM t_user
                         WHERE (@username = '' OR username ILIKE @username)
                         AND (@email = '' OR email ILIKE @email)
                         AND (@mobile = '' OR mobile ILIKE @mobile)
                         AND (@gender = '' OR gender = @gender)
                         AND (@city = '' OR city = @city)
                         ORDER BY user_id";

            using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue(
                    "@username",
                    string.IsNullOrEmpty(username)
                        ? ""
                        : "%" + username + "%"
                );

                cmd.Parameters.AddWithValue(
                    "@email",
                    string.IsNullOrEmpty(email)
                        ? ""
                        : "%" + email + "%"
                );

                cmd.Parameters.AddWithValue(
                    "@mobile",
                    string.IsNullOrEmpty(mobile)
                        ? ""
                        : "%" + mobile + "%"
                );

                cmd.Parameters.AddWithValue(
                    "@gender",
                    gender ?? ""
                );

                cmd.Parameters.AddWithValue(
                    "@city",
                    city ?? ""
                );

                await con.OpenAsync();

                using (NpgsqlDataReader reader =
                       await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        t_user user = new t_user
                        {
                            user_id = Convert.ToInt32(reader["user_id"]),
                            username = reader["username"].ToString(),
                            email = reader["email"].ToString(),
                            mobile = reader["mobile"].ToString(),
                            gender = reader["gender"].ToString(),
                            city = reader["city"].ToString()
                        };

                        users.Add(user);
                    }
                }

                await con.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in SearchUsers");
            Console.WriteLine(ex.Message);

            if (con.State == System.Data.ConnectionState.Open)
            {
                await con.CloseAsync();
            }
        }

        return users;
    }
}
