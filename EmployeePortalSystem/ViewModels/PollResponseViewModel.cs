﻿namespace EmployeePortalSystem.ViewModels
{
    public class PollResponseViewModel
    {
        public int PollResponseId { get; set; }
        public int PollId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string SelectedOption { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
