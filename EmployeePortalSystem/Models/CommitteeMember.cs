namespace EmployeePortalSystem.Models
{
    public class CommitteeMember
    {
        public int CommitteeMemberId { get; set; }
        public int CommitteeId { get; set; }
        public int EmployeeId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
