namespace EmployeePortalSystem.Models
{
    public class Committee
    {
        public int CommitteeId { get; set; }

        public string Name { get; set; }
        public string Type { get; set; }

        public int? HeadId { get; set; } // EmployeeId of the head of the committee

        public string? Logo { get; set; } // URL or path to the committee logo
        public string? Description { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}