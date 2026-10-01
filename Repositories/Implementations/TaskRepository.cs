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

    public async Task<List<t_task>> SearchTasks(
    int user_id,
    string search,
    string category,
    string priority,
    string status,
    DateTime? due_date)
    {
        List<t_task> tasks = new List<t_task>();

        try
        {
            string query = @"SELECT task_id, user_id, task_name, category,
                                priority, due_date, status
                         FROM t_task
                         WHERE user_id = @user_id
                         AND (@search = '' OR task_name ILIKE @search)
                         AND (@category = '' OR category = @category)
                         AND (@priority = '' OR priority = @priority)
                         AND (@status = '' OR status = @status)
                         AND (CAST(@due_date AS date) IS NULL
                              OR due_date = CAST(@due_date AS date))";

            using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@user_id", user_id);

                cmd.Parameters.AddWithValue(
                    "@search",
                    string.IsNullOrEmpty(search) ? "" : "%" + search + "%"
                );

                cmd.Parameters.AddWithValue(
                    "@category",
                    category ?? ""
                );

                cmd.Parameters.AddWithValue(
                    "@priority",
                    priority ?? ""
                );

                cmd.Parameters.AddWithValue(
                    "@status",
                    status ?? ""
                );

                cmd.Parameters.Add("@due_date", NpgsqlTypes.NpgsqlDbType.Date)
                              .Value = due_date.HasValue
                                  ? DateOnly.FromDateTime(due_date.Value)
                                  : DBNull.Value;

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
                            due_date = ((DateOnly)reader["due_date"])
                                .ToDateTime(TimeOnly.MinValue),
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
            Console.WriteLine("Error in SearchTasks");
            Console.WriteLine(ex.Message);

            if (con.State == System.Data.ConnectionState.Open)
            {
                await con.CloseAsync();
            }
        }

        return tasks;
    }

    public async Task<List<t_task>> GetAllTasks()
    {
        List<t_task> tasks = new List<t_task>();

        try
        {
            string query = @"SELECT task_id, user_id, task_name,
                                category, priority, due_date, status
                         FROM t_task
                         ORDER BY task_id";

            using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
            {
                await con.OpenAsync();

                using (NpgsqlDataReader reader =
                       await cmd.ExecuteReaderAsync())
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
                            due_date = ((DateOnly)reader["due_date"])
                                .ToDateTime(TimeOnly.MinValue),
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
            Console.WriteLine("Error in GetAllTasks");
            Console.WriteLine(ex.Message);

            if (con.State == System.Data.ConnectionState.Open)
            {
                await con.CloseAsync();
            }
        }

        return tasks;
    }

    public async Task<vm_AdminDashboard> GetDashboardSummary()
    {
        vm_AdminDashboard dashboard = new vm_AdminDashboard();

        try
        {
            string query = @"SELECT
                            COUNT(*) AS total_tasks,
                            COUNT(*) FILTER (WHERE status = 'Completed') AS completed_tasks,
                            COUNT(*) FILTER (WHERE status = 'Pending') AS pending_tasks,
                            COUNT(*) FILTER (WHERE status = 'In Progress') AS in_progress_tasks
                         FROM t_task";

            using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
            {
                await con.OpenAsync();

                using (NpgsqlDataReader reader =
                       await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        dashboard.TotalTasks =
                            Convert.ToInt32(reader["total_tasks"]);

                        dashboard.CompletedTasks =
                            Convert.ToInt32(reader["completed_tasks"]);

                        dashboard.PendingTasks =
                            Convert.ToInt32(reader["pending_tasks"]);

                        dashboard.InProgressTasks =
                            Convert.ToInt32(reader["in_progress_tasks"]);
                    }
                }

                await con.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in GetDashboardSummary");
            Console.WriteLine(ex.Message);

            if (con.State == System.Data.ConnectionState.Open)
            {
                await con.CloseAsync();
            }
        }

        return dashboard;
    }

    public async Task<List<t_task>> SearchAllTasks(
    string task_name,
    int? user_id,
    string category,
    string priority,
    string status,
    DateTime? due_date)
    {
        List<t_task> tasks = new List<t_task>();

        try
        {
            string query = @"SELECT task_id, user_id, task_name,
                                category, priority, due_date, status
                         FROM t_task
                         WHERE (@task_name = ''
                                OR task_name ILIKE @task_name)
                         AND (@user_id IS NULL
                              OR user_id = @user_id)
                         AND (@category = ''
                              OR category = @category)
                         AND (@priority = ''
                              OR priority = @priority)
                         AND (@status = ''
                              OR status = @status)
                         AND (CAST(@due_date AS date) IS NULL
                              OR due_date = CAST(@due_date AS date))
                         ORDER BY task_id";

            using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue(
                    "@task_name",
                    string.IsNullOrEmpty(task_name)
                        ? ""
                        : "%" + task_name + "%"
                );

                cmd.Parameters.Add(
                    "@user_id",
                    NpgsqlTypes.NpgsqlDbType.Integer
                ).Value = user_id.HasValue
                    ? user_id.Value
                    : DBNull.Value;

                cmd.Parameters.AddWithValue(
                    "@category",
                    category ?? ""
                );

                cmd.Parameters.AddWithValue(
                    "@priority",
                    priority ?? ""
                );

                cmd.Parameters.AddWithValue(
                    "@status",
                    status ?? ""
                );

                cmd.Parameters.Add(
                    "@due_date",
                    NpgsqlTypes.NpgsqlDbType.Date
                ).Value = due_date.HasValue
                    ? DateOnly.FromDateTime(due_date.Value)
                    : DBNull.Value;

                await con.OpenAsync();

                using (NpgsqlDataReader reader =
                       await cmd.ExecuteReaderAsync())
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
                            due_date = ((DateOnly)reader["due_date"])
                                .ToDateTime(TimeOnly.MinValue),
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
            Console.WriteLine("Error in SearchAllTasks");
            Console.WriteLine(ex.Message);

            if (con.State == System.Data.ConnectionState.Open)
            {
                await con.CloseAsync();
            }
        }

        return tasks;
    }

    public async Task<List<vm_UserTaskSummary>> GetUserTaskSummary()
    {
        List<vm_UserTaskSummary> summary = new List<vm_UserTaskSummary>();

        try
        {
            string query = @"SELECT
                            u.user_id,
                            u.username,
                            COUNT(t.task_id) AS total_tasks
                         FROM t_user u
                         LEFT JOIN t_task t
                         ON u.user_id = t.user_id
                         GROUP BY u.user_id, u.username
                         ORDER BY u.user_id";

            using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
            {
                await con.OpenAsync();

                using (NpgsqlDataReader reader =
                       await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        vm_UserTaskSummary data = new vm_UserTaskSummary
                        {
                            user_id = Convert.ToInt32(reader["user_id"]),
                            username = reader["username"].ToString(),
                            total_tasks = Convert.ToInt32(reader["total_tasks"])
                        };

                        summary.Add(data);
                    }
                }

                await con.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in GetUserTaskSummary");
            Console.WriteLine(ex.Message);

            if (con.State == System.Data.ConnectionState.Open)
                await con.CloseAsync();
        }

        return summary;
    }

    public async Task<List<vm_TaskStatusReport>> GetTaskStatusReport()
    {
        List<vm_TaskStatusReport> report =
            new List<vm_TaskStatusReport>();

        try
        {
            string query = @"SELECT statuses.status,
                        COUNT(t.task_id) AS total_tasks
                 FROM (
                     VALUES
                         ('Pending'),
                         ('In Progress'),
                         ('Completed'),
                         ('Cancelled')
                 ) AS statuses(status)
                 LEFT JOIN t_task t
                 ON t.status = statuses.status
                 GROUP BY statuses.status
                 ORDER BY
                     CASE statuses.status
                         WHEN 'Pending' THEN 1
                         WHEN 'In Progress' THEN 2
                         WHEN 'Completed' THEN 3
                         WHEN 'Cancelled' THEN 4
                     END";

            using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
            {
                await con.OpenAsync();

                using (NpgsqlDataReader reader =
                       await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        vm_TaskStatusReport data = new vm_TaskStatusReport
                        {
                            status = reader["status"].ToString(),
                            total_tasks = Convert.ToInt32(reader["total_tasks"])
                        };

                        report.Add(data);
                    }
                }

                await con.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in GetTaskStatusReport");
            Console.WriteLine(ex.Message);

            if (con.State == System.Data.ConnectionState.Open)
                await con.CloseAsync();
        }

        return report;
    }

    public async Task<List<vm_OverdueTaskReport>> GetOverdueTaskReport()
    {
        List<vm_OverdueTaskReport> report =
            new List<vm_OverdueTaskReport>();

        try
        {
            string query = @"SELECT
                            u.username,
                            t.task_name,
                            t.due_date,
                            t.priority,
                            t.status
                         FROM t_task t
                         INNER JOIN t_user u
                         ON t.user_id = u.user_id
                         WHERE t.due_date < CURRENT_DATE
                         AND t.status <> 'Completed'
                         ORDER BY t.due_date";

            using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
            {
                await con.OpenAsync();

                using (NpgsqlDataReader reader =
                       await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        vm_OverdueTaskReport data =
                            new vm_OverdueTaskReport
                            {
                                username = reader["username"].ToString(),
                                task_name = reader["task_name"].ToString(),
                                due_date = ((DateOnly)reader["due_date"])
                                    .ToDateTime(TimeOnly.MinValue),
                                priority = reader["priority"].ToString(),
                                status = reader["status"].ToString()
                            };

                        report.Add(data);
                    }
                }

                await con.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in GetOverdueTaskReport");
            Console.WriteLine(ex.Message);

            if (con.State == System.Data.ConnectionState.Open)
            {
                await con.CloseAsync();
            }
        }

        return report;
    }

    public async Task<List<vm_TasksDueOnDate>> GetTasksDueOnDate(DateTime due_date)
    {
        List<vm_TasksDueOnDate> report =
            new List<vm_TasksDueOnDate>();

        try
        {
            string query = @"SELECT
                            u.username,
                            t.task_name,
                            t.priority,
                            t.status
                         FROM t_task t
                         INNER JOIN t_user u
                         ON t.user_id = u.user_id
                         WHERE t.due_date = @due_date
                         ORDER BY u.username";

            using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.Add("@due_date", NpgsqlTypes.NpgsqlDbType.Date)
                    .Value = DateOnly.FromDateTime(due_date);

                await con.OpenAsync();

                using (NpgsqlDataReader reader =
                       await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        vm_TasksDueOnDate data =
                            new vm_TasksDueOnDate
                            {
                                username = reader["username"].ToString(),
                                task_name = reader["task_name"].ToString(),
                                priority = reader["priority"].ToString(),
                                status = reader["status"].ToString()
                            };

                        report.Add(data);
                    }
                }

                await con.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in GetTasksDueOnDate");
            Console.WriteLine(ex.Message);

            if (con.State == System.Data.ConnectionState.Open)
            {
                await con.CloseAsync();
            }
        }

        return report;
    }
    public async Task<List<vm_UserTaskDetail>> GetUserTaskDetail(int user_id)
    {
        List<vm_UserTaskDetail> report =
            new List<vm_UserTaskDetail>();

        try
        {
            string query = @"SELECT
                            task_name,
                            category,
                            priority,
                            due_date,
                            status
                         FROM t_task
                         WHERE user_id = @user_id
                         ORDER BY due_date";

            using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@user_id", user_id);

                await con.OpenAsync();

                using (NpgsqlDataReader reader =
                       await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        vm_UserTaskDetail data =
                            new vm_UserTaskDetail
                            {
                                task_name = reader["task_name"].ToString(),
                                category = reader["category"].ToString(),
                                priority = reader["priority"].ToString(),
                                due_date = ((DateOnly)reader["due_date"])
                                    .ToDateTime(TimeOnly.MinValue),
                                status = reader["status"].ToString()
                            };

                        report.Add(data);
                    }
                }

                await con.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in GetUserTaskDetail");
            Console.WriteLine(ex.Message);

            if (con.State == System.Data.ConnectionState.Open)
            {
                await con.CloseAsync();
            }
        }

        return report;
    }
}