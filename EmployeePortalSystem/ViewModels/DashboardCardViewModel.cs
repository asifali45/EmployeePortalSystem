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

        public Dictionary<string, int> DepartmentMemberCounts { get; set; } = new();

        public List<ContributorStats> TopContributors { get; set; } 
    }
    public class ContributorStats
    {
        public string EmployeeName { get; set; }
        public int Blogs { get; set; }
        public int Polls { get; set; }
        public int Awards { get; set; }
        public int Total => Blogs + Polls + Awards;
    }
}
