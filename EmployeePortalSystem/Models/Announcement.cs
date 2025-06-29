using System.ComponentModel.DataAnnotations;

namespace EmployeePortalSystem.Models
{
    public class Announcement
    {
        public int AnnouncementId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime? PostDate { get; set; }
        public string VisibleTo { get; set; } // enum: All, Department, Committee
        public int? DisplayOrder { get; set; }
        public bool IsEvent { get; set; }
        public DateTime? EventDate { get; set; }
        public TimeSpan? EventTime { get; set; }
        public string? Location { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
