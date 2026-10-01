using Npgsql;
using Repositories.Models;

namespace Repositories;

public class AdminRepository : IAdminInterface
{
    private readonly NpgsqlConnection con;

    public AdminRepository(NpgsqlConnection connection)
    {
        con = connection;
    }

    public async Task<t_admin> Login(vm_Login login)
    {
        t_admin admin = null;

        try
        {
            string query = @"SELECT user_id, email, password
                             FROM t_admin
                             WHERE email = @email
                             AND password = @password";

            using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@email", login.email);
                cmd.Parameters.AddWithValue("@password", login.password);

                await con.OpenAsync();

                using (NpgsqlDataReader reader =
                       await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        admin = new t_admin
                        {
                            user_id = Convert.ToInt32(reader["user_id"]),
                            email = reader["email"].ToString(),
                            password = reader["password"].ToString()
                        };
                    }
                }

                await con.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in Admin Login");
            Console.WriteLine(ex.Message);

            if (con.State == System.Data.ConnectionState.Open)
            {
                await con.CloseAsync();
            }
        }

        return admin;
    }
}