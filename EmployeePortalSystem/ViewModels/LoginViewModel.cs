using System.ComponentModel.DataAnnotations;

namespace EmployeePortalSystem.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(4, ErrorMessage = "Password must be at least 4 characters")]
        public string Password { get; set; }
        public int EmployeeId { get; set; }

        public string? Name { get; set; }
        public string? RoleName { get; set; }
        public bool IsAdmin { get; set; }
    }
}
