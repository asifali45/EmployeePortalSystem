﻿using EmployeePortalSystem.Models;
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

        [Required]
        public string Status { get; set; } = "Open";

        public string Response { get; set; } = string.Empty;

        //  Assignment & Escalation
        public string AssignedTo { get; set; } = string.Empty; //  selected EmployeeId
        public string EscalationName { get; set; } = string.Empty; // selected EscalatedTo Id
        public string EscalationLevel { get; set; } = string.Empty;

        public string AssignedToName { get; set; } = string.Empty;
        public string EscalatedToName { get; set; } = string.Empty;
        public string EscalationLevelName { get; set; } = string.Empty;

        //  Employee Info
        public string EmployeeName { get; set; } = string.Empty;

        // Dropdown Options
        public int DepartmentId { get; set; }
        public List<SelectListItem> DepartmentList { get; set; } = new();
        public List<SelectListItem> FilteredEmployees { get; set; } = new();

        //  only if used in RaiseTicket
        public List<string> EmployeeNameList { get; set; } = new();
    }
}
