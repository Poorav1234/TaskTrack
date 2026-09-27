using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Models;

public class t_user
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int user_id {get; set; }

    [StringLength(100)]
    [Required(ErrorMessage = "Username is required")]
    public string username {get; set; }

    [StringLength(100)]
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string email {get; set; }

    [StringLength(10)]
    [Required(ErrorMessage = "Mobile number is required")]
    [RegularExpression(@"^[6-9][0-9]{9}$", ErrorMessage = "Invalid mobile number")]

    public string mobile {get; set; }

    [StringLength(10)]
    [Required(ErrorMessage = "Gender is required")]
    public string gender {get; set; }

    [StringLength(100)]
    [Required(ErrorMessage = "City is required")]
    public string city {get; set; }

    [StringLength(100)]
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string password {get; set; }

    [StringLength(100)]
    [Required(ErrorMessage = "Confirm Password is required")]
    [Compare("password", ErrorMessage = "Passwords do not match")]
    public string confirm_password {get; set; }
}
