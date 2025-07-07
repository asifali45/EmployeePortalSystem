using System.ComponentModel.DataAnnotations;

namespace EmployeePortalSystem.Models
{
    public class Poll
    {
        public int PollId { get; set; }

        [Required(ErrorMessage = "Question is required")]
        public string Question { get; set; }

        [Required(ErrorMessage = "Option 1 is required")]
        public string Option1 { get; set; }

        [Required(ErrorMessage = "Option 2 is required")]
        public string Option2 { get; set; }

        public string? Option3 { get; set; }  // ✅ nullable

        public string? Option4 { get; set; }  // ✅ nullable

        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }
    }

}

