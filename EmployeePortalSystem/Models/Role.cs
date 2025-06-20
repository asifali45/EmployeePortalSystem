namespace EmployeePortalSystem.Models
{
    public class Role
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
