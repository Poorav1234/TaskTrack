using Npgsql;
using Repositories.Models;

namespace Repositories;

public class TaskRepository : ITaskInterface
{
    private readonly NpgsqlConnection con;

    public TaskRepository(NpgsqlConnection connection)
    {
        con = connection;
    }

    public async Task<int> CreateTask(t_task task)
    {
        try
        {
            string query = @"INSERT INTO t_task
                            (user_id, task_name, category, priority, due_date, status)
                            VALUES
                            (@user_id, @task_name, @category, @priority, @due_date, @status)";

            using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@user_id", task.user_id);
                cmd.Parameters.AddWithValue("@task_name", task.task_name);
                cmd.Parameters.AddWithValue("@category", task.category);
                cmd.Parameters.AddWithValue("@priority", task.priority);
                cmd.Parameters.AddWithValue("@due_date", task.due_date);
                cmd.Parameters.AddWithValue("@status", task.status);

                await con.OpenAsync();

                int result = await cmd.ExecuteNonQueryAsync();

                await con.CloseAsync();

                return result > 0 ? 1 : -1;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in CreateTask");
            Console.WriteLine(ex.Message);

            if (con.State == System.Data.ConnectionState.Open)
            {
                await con.CloseAsync();
            }

            return -1;
        }
    }


    public async Task<t_task> GetTask(int task_id)
    {
        t_task task = null;

        try
        {
            string query = @"SELECT task_id, user_id, task_name, category,
                                    priority, due_date, status
                             FROM t_task
                             WHERE task_id = @task_id";

            using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@task_id", task_id);

                await con.OpenAsync();

                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        task = new t_task
                        {
                            task_id = Convert.ToInt32(reader["task_id"]),
                            user_id = Convert.ToInt32(reader["user_id"]),
                            task_name = reader["task_name"].ToString(),
                            category = reader["category"].ToString(),
                            priority = reader["priority"].ToString(),
                            due_date = ((DateOnly)reader["due_date"]).ToDateTime(TimeOnly.MinValue),
                            status = reader["status"].ToString()
                        };
                    }
                }

                await con.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in GetTask");
            Console.WriteLine(ex.Message);

            if (con.State == System.Data.ConnectionState.Open)
            {
                await con.CloseAsync();
            }
        }

        return task;
    }


    public async Task<List<t_task>> GetTasksByUser(int user_id)
    {
        List<t_task> tasks = new List<t_task>();

        try
        {
            string query = @"SELECT task_id, user_id, task_name, category,
                                    priority, due_date, status
                             FROM t_task
                             WHERE user_id = @user_id";

            using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@user_id", user_id);

                await con.OpenAsync();

                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        t_task task = new t_task
                        {
                            task_id = Convert.ToInt32(reader["task_id"]),
                            user_id = Convert.ToInt32(reader["user_id"]),
                            task_name = reader["task_name"].ToString(),
                            category = reader["category"].ToString(),
                            priority = reader["priority"].ToString(),
                            due_date = ((DateOnly)reader["due_date"]).ToDateTime(TimeOnly.MinValue),
                            status = reader["status"].ToString()
                        };

                        tasks.Add(task);
                    }
                }

                await con.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in GetTasksByUser");
            Console.WriteLine(ex.Message);

            if (con.State == System.Data.ConnectionState.Open)
            {
                await con.CloseAsync();
            }
        }

        return tasks;
    }


    public async Task<int> UpdateTask(t_task task)
    {
        try
        {
            string query = @"UPDATE t_task
                             SET user_id = @user_id,
                                 task_name = @task_name,
                                 category = @category,
                                 priority = @priority,
                                 due_date = @due_date,
                                 status = @status
                             WHERE task_id = @task_id";

            using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@task_id", task.task_id);
                cmd.Parameters.AddWithValue("@user_id", task.user_id);
                cmd.Parameters.AddWithValue("@task_name", task.task_name);
                cmd.Parameters.AddWithValue("@category", task.category);
                cmd.Parameters.AddWithValue("@priority", task.priority);
                cmd.Parameters.AddWithValue("@due_date", task.due_date);
                cmd.Parameters.AddWithValue("@status", task.status);

                await con.OpenAsync();

                int result = await cmd.ExecuteNonQueryAsync();

                await con.CloseAsync();

                return result > 0 ? 1 : 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in UpdateTask");
            Console.WriteLine(ex.Message);

            if (con.State == System.Data.ConnectionState.Open)
            {
                await con.CloseAsync();
            }

            return -1;
        }
    }


    public async Task<int> DeleteTask(int task_id)
    {
        try
        {
            string query = @"DELETE FROM t_task
                             WHERE task_id = @task_id";

            using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@task_id", task_id);

                await con.OpenAsync();

                int result = await cmd.ExecuteNonQueryAsync();

                await con.CloseAsync();

                return result > 0 ? 1 : 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in DeleteTask");
            Console.WriteLine(ex.Message);

            if (con.State == System.Data.ConnectionState.Open)
            {
                await con.CloseAsync();
            }

            return -1;
        }
    }
}