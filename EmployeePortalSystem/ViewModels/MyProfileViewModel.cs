namespace EmployeePortalSystem.ViewModels
{
    public class MyProfileViewModel
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Designation { get; set; }
        public string DepartmentName { get; set; }
        public string? Photo { get; set; }

        public List<string>? Committees { get; set; }
        public List<AwardViewModel>? Awards { get; set; }
        public List<BlogViewModel>? Blogs { get; set; }
        public List<PollViewModel>? Polls { get; set; }
    }
}
