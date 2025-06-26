using System.ComponentModel.DataAnnotations;

namespace EmployeePortalSystem.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Department Name is required")]
        public string Name { get; set; }

        public int? ParentDepartmentId { get; set; }

        [Required(ErrorMessage = "Please select a Head from suggestions")]
        public int? HeadId { get; set; }


        [Required(ErrorMessage = "Description is required")]
        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters")]
        public string? Description { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
