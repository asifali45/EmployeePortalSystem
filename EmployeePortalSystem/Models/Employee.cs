namespace EmployeePortalSystem.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public string Phone { get; set; }
        public string? Photo { get; set; }
        public int RoleId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? DepartmentId { get; set; }
        public bool IsAdmin { get; set; }
        
    }
}
