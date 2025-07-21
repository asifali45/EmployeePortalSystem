namespace EmployeePortalSystem.Models
{
    public class SupportTicket
    {
        public int TicketId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;

        public string IssueTitle { get; set; } = string.Empty;
        public string? Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Response { get; set; }
        public DateTime CreatedAt { get; set; }

        public int? AssignedTo { get; set; }    // FK ID
        public int? EscalatedTo { get; set; }   // FK ID
        public int EscalationLevel { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? Resolved { get; set; }




        public string? AssignedToName { get; set; }
        public string? EscalatedToName { get; set; }
        public int? EscalatedBy { get; set; }
        public string? EscalatedByName { get; set; }

      



    }

}
