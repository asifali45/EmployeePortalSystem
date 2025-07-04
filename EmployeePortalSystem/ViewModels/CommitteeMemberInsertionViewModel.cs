using EmployeePortalSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace EmployeePortalSystem.ViewModels
{
    public class CommitteeMemberInsertionViewModel
    {
        public int CommitteeId { get; set; }

        [Required]
        public int EmployeeId { get; set; }


        public List<Employee>? Employees { get; set; }
        public string? CommitteeName { get; set; }
    }
}
