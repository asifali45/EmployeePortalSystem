namespace EmployeePortalSystem.ViewModels
{
    public class SupportTicketViewModel
    {
        public int TicketId { get; set; }
        public string IssueTitle { get; set; }
        public string Description { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public string? Response { get; set; }
        public string? EmployeeName { get; set; }
        public int EmployeeId { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<SupportTicketViewModel>? TicketList { get; set; }
    }
}
