namespace Repositories.Models;

public class vm_UserTaskDetail
{
    public string task_name { get; set; }
    public string category { get; set; }
    public string priority { get; set; }
    public DateTime due_date { get; set; }
    public string status { get; set; }
}