namespace EmployeePortalSystem.Models
{
    public class Award
    {
        public int AwardId { get; set; }
        public string Type { get; set; }
        public DateTime EventDate { get; set; }
        public int RecipientId { get; set; }
        public string RecipientName { get; set; }
        public string GivenBy { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
