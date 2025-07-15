using System.ComponentModel.DataAnnotations;
namespace EmployeePortalSystem.Models
{
    public class Role
    {
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string RoleName { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
