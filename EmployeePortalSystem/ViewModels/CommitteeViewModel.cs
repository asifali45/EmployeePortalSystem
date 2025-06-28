namespace EmployeePortalSystem.ViewModels
{
    public class CommitteeViewModel
    {
        public int CommitteeId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string? HeadName { get; set; } // From Employee table
        public string? Logo { get; set; }
        public string? Description { get; set; }
    }
}
