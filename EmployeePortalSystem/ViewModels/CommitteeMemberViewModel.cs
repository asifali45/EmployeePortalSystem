namespace EmployeePortalSystem.ViewModels
{
    public class CommitteeMemberViewModel
    {
        public int CommitteeMemberId { get; set; }
        public int CommitteeId { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public string? Photo { get; set; }

    }
}
