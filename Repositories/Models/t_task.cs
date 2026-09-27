using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Models;

public class t_task
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int task_id {get; set; }

    [Required(ErrorMessage = "User ID is required")]
    public int user_id {get; set; }

    [StringLength(100)]
    [Required(ErrorMessage = "Task name is required")]
    public string task_name {get; set; }

    [StringLength(20)]
    [Required(ErrorMessage = "Task Category is required")]
    public string category {get; set; }

    [StringLength(50)]
    [Required(ErrorMessage = "Task priority is required")]
    public string priority {get; set; }

    [Required(ErrorMessage = "Due date is required")]
    public DateTime due_date {get; set; }

    [StringLength(50)]
    [Required(ErrorMessage = "Task status is required")]
    public string status {get; set; }
}
