using EmployeePortalSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EmployeePortalSystem.ViewModels
{
    public class SupportTicketViewModel
    {
        // Ticket Info
        public int TicketId { get; set; }

        [Required]
        public string IssueTitle { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Type is required")]
        public string Type { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Open";

        public string Response { get; set; } = string.Empty;

        //  Assignment & Escalation
        public string AssignedTo { get; set; } = string.Empty; //  selected EmployeeId
        public string EscalationName { get; set; } = string.Empty; // selected EscalatedTo Id
        public string EscalationLevel { get; set; } = "0";

        public string? AssignedToName { get; set; } = string.Empty;
        public string? EscalatedToName { get; set; }
        public string EscalationLevelName { get; set; } = string.Empty;

        //  Employee Info
        public string EmployeeName { get; set; } = string.Empty;



        // Dropdown Options
        public int DepartmentId { get; set; }
        public List<SelectListItem>? DepartmentList { get; set; }

        public List<SupportEmployeeListItem> AllEmployees { get; set; } = new();

        public List<SelectListItem> FilteredEmployees { get; set; } = new();

        //  only if used in RaiseTicket
        public List<string> EmployeeNameList { get; set; } = new();
        public string? Resolved { get; set; }
        public int? EscalatedBy { get; set; }
        public string? EscalatedByName { get; set; }

       



    }
    public class SupportEmployeeListItem
    {
        public string Name { get; set; }
        public int DepartmentId { get; set; }
    }
}
