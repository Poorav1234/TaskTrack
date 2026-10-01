using Repositories.Models;

namespace Repositories;

public interface ITaskInterface
{
    Task<int> CreateTask(t_task task);
    Task<t_task> GetTask(int task_id);
    Task<List<t_task>> GetTasksByUser(int user_id);
    Task<int> UpdateTask(t_task task);
    Task<int> DeleteTask(int task_id);

    Task<List<t_task>> SearchTasks(
        int user_id,
        string search,
        string category,
        string priority,
        string status,
        DateTime? due_date
    );

    Task<List<t_task>> SearchAllTasks(
    string task_name,
    int? user_id,
    string category,
    string priority,
    string status,
    DateTime? due_date);

    Task<List<t_task>> GetAllTasks();

    Task<vm_AdminDashboard> GetDashboardSummary();

    Task<List<vm_UserTaskSummary>> GetUserTaskSummary();
    Task<List<vm_TaskStatusReport>> GetTaskStatusReport();
    Task<List<vm_OverdueTaskReport>> GetOverdueTaskReport();
    Task<List<vm_TasksDueOnDate>> GetTasksDueOnDate(DateTime due_date);
    Task<List<vm_UserTaskDetail>> GetUserTaskDetail(int user_id);

}
