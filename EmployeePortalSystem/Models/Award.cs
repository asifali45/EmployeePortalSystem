namespace EmployeePortalSystem.Models
{
    public class Award
    {
        public int AwardId { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime? EventDate { get; set; }
        public int RecipientId { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string GivenBy { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string RecipientPhoto { get; set; } = string.Empty;
    }
}
