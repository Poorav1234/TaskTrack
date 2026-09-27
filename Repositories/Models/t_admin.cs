using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Models;

public class t_admin
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int admin_id { get; set; }

    [StringLength(100)]
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string email { get; set; }

    [StringLength(100)]
    [Required(ErrorMessage = "Password is required")]
    public string password { get; set; }
}