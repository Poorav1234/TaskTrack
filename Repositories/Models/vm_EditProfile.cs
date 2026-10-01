using System.ComponentModel.DataAnnotations;

namespace Repositories.Models;

public class vm_EditProfile
{
    [StringLength(100)]
    [Required(ErrorMessage = "Username is required")]
    public string username { get; set; }

    [StringLength(100)]
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string email { get; set; }

    [StringLength(10)]
    [Required(ErrorMessage = "Mobile number is required")]
    [RegularExpression(@"^[6-9][0-9]{9}$", ErrorMessage = "Invalid mobile number")]
    public string mobile { get; set; }

    [StringLength(10)]
    [Required(ErrorMessage = "Gender is required")]
    public string gender { get; set; }

    [StringLength(100)]
    [Required(ErrorMessage = "City is required")]
    public string city { get; set; }
}