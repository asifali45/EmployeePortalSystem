namespace EmployeePortalSystem.Models
{
    public class CommitteeMember
    {
        public int CommitteeMemberId { get; set; }
        public int CommitteeId { get; set; }
        public int EmployeeId { get; set; }
        public int RoleId { get; set; }
        public int CreatedBy { get; set; }
        public int CreatedAt { get; set; }
        public int UpdatedBy { get; set; }
        public int UpdatedAt { get; set; }
    }
}
