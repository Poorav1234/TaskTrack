using Repositories.Models;

namespace Repositories;

public interface ITaskInterface
{
    Task<int> CreateTask(t_task task);
    Task<t_task> GetTask(int task_id);
    Task<List<t_task>> GetTasksByUser(int user_id);
    Task<int> UpdateTask(t_task task);
    Task<int> DeleteTask(int task_id);
}
