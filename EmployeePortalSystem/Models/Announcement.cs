using System.ComponentModel.DataAnnotations;

namespace EmployeePortalSystem.Models
{
    public class Announcement
    {
        public int AnnouncementId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        public string? Message { get; set; }

        [Required(ErrorMessage = "VisibleTo is required")]
        public string VisibleTo { get; set; }

        [Required(ErrorMessage = "Display Order is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Display Order must be greater than 0")]
        public int DisplayOrder { get; set; }

        [Required(ErrorMessage = "IsEvent selection is required")]
        public bool? IsEvent { get; set; }

        public DateTime? PostDate { get; set; }
        public DateTime? EventDate { get; set; }
        public TimeSpan? EventTime { get; set; }
        public string? Location { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
