using System.ComponentModel.DataAnnotations;

namespace EmployeePortalSystem.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        public string? Password { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone must be 10 digits")]
        public string Phone { get; set; }
        public string? Photo { get; set; }

        [Required(ErrorMessage = "Designation is required")]
        public int RoleId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public int? DepartmentId { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsCurrentEmployee { get; set; }
        
    }
}
