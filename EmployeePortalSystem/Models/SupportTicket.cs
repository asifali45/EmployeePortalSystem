namespace EmployeePortalSystem.Models
{
    public class SupportTicket
    {
        public int TicketId { get; set; }
        public int EmployeeId { get; set; }
        public string IssueTitle { get; set; }
        public string? Type { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string? Response { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? AssignedTo { get; set; }
        public int? EscalatedTo { get; set; }
        public int EscalationLevel { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
