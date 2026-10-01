namespace Repositories.Models;

public class vm_OverdueTaskReport
{
    public string username { get; set; }
    public string task_name { get; set; }
    public DateTime due_date { get; set; }
    public string priority { get; set; }
    public string status { get; set; }
}