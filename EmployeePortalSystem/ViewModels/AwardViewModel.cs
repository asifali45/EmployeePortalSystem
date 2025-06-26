using EmployeePortalSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace EmployeePortalSystem.ViewModels
{
    public class AwardViewModel
    {
        public int AwardId { get; set; }
        public string Type { get; set; } = string.Empty;

     
        public DateTime? EventDate { get; set; }

        public string RecipientName { get; set; } = string.Empty;
        public string GivenBy { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }

        public List<Award> AwardList { get; set; } = new List<Award>();
    }
}
