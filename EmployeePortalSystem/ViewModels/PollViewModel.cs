namespace EmployeePortalSystem.ViewModels
{
    public class PollViewModel
    {
        public int PollId { get; set; }
        public string Question { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }  // from Employee table
        public DateTime CreatedAt { get; set; }
    }
}
