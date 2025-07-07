namespace EmployeePortalSystem.ViewModels
{
    public class EmployeeAwardViewModel
    {
        public string Type { get; set; }
        public DateTime EventDate { get; set; }
        public string RecipientName { get; set; }
        public string GivenBy { get; set; }
        public string Description { get; set; }
        public string RecipientPhoto { get; set; }  // filename only
    }

}
