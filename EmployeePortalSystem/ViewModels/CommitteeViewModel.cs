namespace EmployeePortalSystem.ViewModels
{
    public class CommitteeViewModel
    {
        public int CommitteeId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int? HeadId { get; set; }
        public string? HeadName { get; set; } // From Employee table
        public IFormFile? LogoFile { get; set; } // for upload only
        public string? Logo { get; set; }
        public string? Description { get; set; }
    }
}
