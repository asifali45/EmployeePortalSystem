using EmployeePortalSystem.Models;

namespace EmployeePortalSystem.ViewModels
{
    public class DashboardCardViewModel
    {
        public int TotalAwards { get; set; }

        public int BlogsWritten { get; set; }

        public int PollsVoted { get; set; }

        // Latest blogs section
        public List<BlogViewModel> LatestBlogs { get; set; }

        public List<Announcement> LatestAnnouncements { get; set; } = new();

        public List<AwardViewModel> LatestAwards { get; set; }

        public List<PollViewModel> LatestPolls { get; set; }

        public Dictionary<string, int> ContributionChartData { get; set; }
        public Dictionary<string, int> MonthlyContributionData { get; set; } = new();
    }
}
