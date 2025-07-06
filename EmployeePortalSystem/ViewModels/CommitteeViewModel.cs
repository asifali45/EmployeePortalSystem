using System.ComponentModel.DataAnnotations;

namespace EmployeePortalSystem.ViewModels
{
    public class CommitteeViewModel
    {
        public int? CommitteeId { get; set; }

        [Required(ErrorMessage="Committee Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please select a type.")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Please select a Head.")]
        [Range(1, int.MaxValue, ErrorMessage = "HeadId must be valid.")]
        public int? HeadId { get; set; }
        public string? HeadName { get; set; } // From Employee table
        public IFormFile? LogoFile { get; set; } // for upload only
        public string? Logo { get; set; }

        [StringLength(250, ErrorMessage = "Description can't exceed 250 characters.")]
        public string? Description { get; set; }

        public int MemberCount { get; set; }
    }
}
