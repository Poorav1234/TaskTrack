using System.ComponentModel.DataAnnotations;

namespace Repositories.Models;

public class vm_ChangePassword
{
    [Required(ErrorMessage = "Old password is required")]
    public string old_password { get; set; }

    [Required(ErrorMessage = "New password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string new_password { get; set; }

    [Required(ErrorMessage = "Confirm password is required")]
    [Compare("new_password", ErrorMessage = "Passwords do not match")]
    public string confirm_password { get; set; }
}