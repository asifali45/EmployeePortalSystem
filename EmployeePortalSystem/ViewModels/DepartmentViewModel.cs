namespace EmployeePortalSystem.ViewModels
{
    public class DepartmentViewModel
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? HeadName { get; set; } // from Employee table
        public string? ParentDepartmentName { get; set; } // from Department table
        public int EmployeeCount { get; set; }
    }
}
