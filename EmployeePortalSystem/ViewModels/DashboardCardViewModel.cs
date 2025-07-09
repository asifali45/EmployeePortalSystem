namespace EmployeePortalSystem.ViewModels
{
    public class DashboardCardViewModel
    {
        public int TotalAwards { get; set; }

        public int BlogsWritten { get; set; }

        public int PollsVoted { get; set; }

        // Latest blogs section
        public List<BlogViewModel> LatestBlogs { get; set; }

    }
}
